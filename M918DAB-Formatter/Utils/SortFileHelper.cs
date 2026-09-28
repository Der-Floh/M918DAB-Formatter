namespace M918DAB_Formatter.Utils;

public static class SortFileHelper
{
    private static readonly string[] ReservedDirNames =
    [
        "System Volume Information", "$RECYCLE.BIN", "RECYCLED", "RECYCLER",
        ".Spotlight-V100", ".Trashes", ".fseventsd", ".TemporaryItems",
        "FOUND.000", "FOUND.001", "LOST.DIR"
    ];

    private static readonly string[] ReservedFileNames =
    [
        ".DS_Store",
        "Thumbs.db",
        "desktop.ini",
        "IndexerVolumeGuid"
    ];

    private static readonly string[] ReservedFilePrefixes =
    [
        "._"
    ];

    private static readonly char[] PathSeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

    private const string StagePrefix = ".reorder_stage_";
    private const int MaxFatNameLength = 255;

    public static int SortFiles(string? root, Action<double, int>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(root))
            throw new ArgumentException("No folder selected.", nameof(root));

        return ReorderByAlphabeticalCopyOrder(root!, true, true, false, true, progressCallback, cancellationToken);
    }

    public static int ReorderByAlphabeticalCopyOrder(string folderPath, bool recursive = false, bool orderDirectoriesToo = false, bool includeHiddenAndSystem = false, bool allowRoot = false, Action<double, int>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentNullException(nameof(folderPath));

        folderPath = NormalizeFolderPath(folderPath);
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException(folderPath);

        var isRoot = IsPathRoot(folderPath);
        if (isRoot && !allowRoot)
            throw new InvalidOperationException("Refusing to reorder the drive root. Set allowRoot:true if you really want this.");

        EnsureDeviceReadableVolume(folderPath);
        RestoreInterruptedSorts(folderPath, isRoot, cancellationToken);

        var counter = new WorkCounter();
        CountWork(folderPath, recursive, includeHiddenAndSystem, counter, cancellationToken);

        var prog = new ProgressState(progressCallback, counter.Total);
        DoFolder(folderPath, recursive, orderDirectoriesToo, includeHiddenAndSystem, prog, cancellationToken);
        prog.Report(forceDone: true);
        return prog.Errors;
    }

    /// <summary>
    /// Makes the path absolute and drops trailing separators, except on a drive root: <c>E:</c> without its
    /// backslash means the current directory on that drive, not the root.
    /// </summary>
    private static string NormalizeFolderPath(string folderPath)
    {
        var fullPath = Path.GetFullPath(folderPath.Trim());
        return IsPathRoot(fullPath) ? fullPath : fullPath.TrimEnd(PathSeparators);
    }

    private static bool IsPathRoot(string path) =>
        string.Equals(path.TrimEnd(PathSeparators), (Path.GetPathRoot(path) ?? string.Empty).TrimEnd(PathSeparators), StringComparison.OrdinalIgnoreCase);

    private static void EnsureDeviceReadableVolume(string folderPath)
    {
        var format = FatDirectory.GetVolumeFormat(folderPath);
        if (FatDirectory.IsDeviceReadable(folderPath))
            return;

        if (string.Equals(format, "exFAT", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException("This drive is formatted as exFAT. The Kenwood M-918DAB only reads FAT16 and FAT32, so it cannot play this drive at all. Reformat it as FAT32 and copy the music back before sorting.");

        throw new NotSupportedException($"Detected filesystem '{format ?? "unknown"}'. Sorting only works on FAT16/FAT32, which is also all the Kenwood M-918DAB can read.");
    }

    /// <summary>
    /// Moves the entries of staging folders left behind by an interrupted run back into the folders they
    /// belong to, before anything else changes. Looks inside the folder being sorted and, for a subfolder,
    /// in its parent, where that subfolder's own staging folder lives.
    /// </summary>
    private static void RestoreInterruptedSorts(string folderPath, bool isRoot, CancellationToken ct)
    {
        var unresolved = new List<string>();
        foreach (var stage in FindStages(folderPath, isRoot))
        {
            ct.ThrowIfCancellationRequested();

            var target = StageTarget(stage);
            if (target is null || !TryRestoreStage(stage, target))
                unresolved.Add(stage);
        }

        if (unresolved.Count > 0)
            throw new IOException(UnresolvedStagesMessage(unresolved));
    }

    private static List<string> FindStages(string folderPath, bool isRoot)
    {
        var stages = new List<string>();
        if (!isRoot)
            stages.AddRange(Directory.GetDirectories(Directory.GetParent(folderPath)!.FullName).Where(IsStage));

        CollectStages(folderPath, stages);
        return stages;
    }

    private static void CollectStages(string dir, List<string> stages)
    {
        foreach (var sub in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
        {
            if (IsStage(sub))
                stages.Add(sub);
            else if (!IsReservedDirectory(sub))
                CollectStages(sub, stages);
        }
    }

    private static string UnresolvedStagesMessage(List<string> stages) =>
        "An earlier sort was interrupted and left staging folders behind that couldn't be moved back automatically:"
        + Environment.NewLine + Environment.NewLine
        + string.Join(Environment.NewLine, stages)
        + Environment.NewLine + Environment.NewLine
        + "They're hidden and hold entries of a folder next to them or of the drive root. Move the entries back where they belong, delete the empty staging folders and sort again (see \"A sort was interrupted\" in the README). Nothing has been sorted yet.";

    private static void DoFolder(string current, bool rec, bool orderDirs, bool includeHidden, ProgressState ps, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (rec)
        {
            foreach (var sub in Directory.GetDirectories(current, "*", SearchOption.TopDirectoryOnly))
            {
                if (IsReservedDirectory(sub))
                    continue;
                DoFolder(sub, true, orderDirs, includeHidden, ps, ct);
            }
        }

        var currentIsRoot = IsPathRoot(current);

        if (currentIsRoot)
            ps.AddErrors(FatDirectoryPacker.RemoveFillers(current));

        var plan = BuildPlan(current, orderDirs, includeHidden);

        var stage = StagePathFor(current, currentIsRoot);
        Directory.CreateDirectory(stage);
        TrySetAttributes(stage, FileAttributes.Hidden | FileAttributes.System);

        try
        {
            MoveAll(plan, stage, ps, ct);
            SweepLeftovers(current, stage, ps);

            if (currentIsRoot)
                RestoreIntoRoot(current, stage, plan, ps, ct);
            else
                ReplaceWithStage(current, stage, ps);
        }
        catch
        {
            TryRestoreStage(stage, current);
            throw;
        }
    }

    /// <summary>
    /// Names the staging folder after the folder it belongs to, so that <see cref="StageTarget"/> can tell
    /// where its entries go after an interruption. The drive root's staging folder sits inside the root and
    /// gets no suffix; names too long for FAT get a random one and can only be restored by hand.
    /// </summary>
    private static string StagePathFor(string current, bool currentIsRoot)
    {
        if (currentIsRoot)
            return Path.Combine(current, StagePrefix);

        var name = Path.GetFileName(current);
        var suffix = StagePrefix.Length + name.Length <= MaxFatNameLength ? name : Guid.NewGuid().ToString("N");
        return Path.Combine(Directory.GetParent(current)!.FullName, StagePrefix + suffix);
    }

    /// <summary>
    /// The folder whose entries <paramref name="stage"/> holds, or <see langword="null"/> for staging folders
    /// with a random suffix, which older versions created for every folder.
    /// </summary>
    private static string? StageTarget(string stage)
    {
        var parent = Path.GetDirectoryName(stage);
        var folderName = Path.GetFileName(stage).Substring(StagePrefix.Length);
        if (Guid.TryParseExact(folderName, "N", out _))
            return null;

        return folderName.Length == 0 ? parent : Path.Combine(parent, folderName);
    }

    /// <summary>
    /// The drive root cannot be deleted and recreated, so its entries are written back into the existing
    /// directory table. Packing the free slots first is what keeps them in order.
    /// </summary>
    private static void RestoreIntoRoot(string current, string stage, List<PlannedEntry> plan, ProgressState ps, CancellationToken ct)
    {
        try
        {
            FatDirectoryPacker.FillFreeSlots(current, ct);

            foreach (var entry in plan)
            {
                ct.ThrowIfCancellationRequested();
                var name = Path.GetFileName(entry.Path);
                TryMove(Path.Combine(stage, name), Path.Combine(current, name), entry.IsDirectory, ps);
            }

            foreach (var leftover in Directory.EnumerateFileSystemEntries(stage).ToArray())
            {
                var name = Path.GetFileName(leftover);
                TryMove(leftover, Path.Combine(current, name), Directory.Exists(leftover), ps);
            }
        }
        finally
        {
            ps.AddErrors(FatDirectoryPacker.RemoveFillers(current));
        }

        TryClearAttributes(stage, FileAttributes.Hidden | FileAttributes.System);
        Directory.Delete(stage, false);
        ps.Step();
    }

    private static void ReplaceWithStage(string current, string stage, ProgressState ps)
    {
        Directory.Delete(current, recursive: false);
        ps.Step();

        TryClearAttributes(stage, FileAttributes.Hidden | FileAttributes.System);
        Directory.Move(stage, current);
        ps.Step();
    }

    private static List<PlannedEntry> BuildPlan(string current, bool orderDirs, bool includeHidden)
    {
        var filesAll = Directory.GetFiles(current, "*", SearchOption.TopDirectoryOnly);
        var dirsAll = Directory.GetDirectories(current, "*", SearchOption.TopDirectoryOnly)
            .Where(d => !IsReservedDirectory(d)).ToArray();

        var (filesMain, filesHidden) = SplitHidden(filesAll, includeHidden);
        var (dirsMain, dirsHidden) = SplitHidden(dirsAll, includeHidden);

        var plan = new List<PlannedEntry>(filesAll.Length + dirsAll.Length);

        if (orderDirs)
        {
            var together = new List<PlannedEntry>(dirsMain.Length + filesMain.Length);
            together.AddRange(dirsMain.Select(d => new PlannedEntry(d, true)));
            together.AddRange(filesMain.Select(f => new PlannedEntry(f, false)));
            plan.AddRange(together.OrderBy(e => Path.GetFileName(e.Path), NaturalFileNameComparer.Instance));
        }
        else
        {
            plan.AddRange(dirsMain.Select(d => new PlannedEntry(d, true)));
            plan.AddRange(filesMain
                .OrderBy(Path.GetFileName, NaturalFileNameComparer.Instance)
                .Select(f => new PlannedEntry(f, false)));
        }

        plan.AddRange(dirsHidden.Select(d => new PlannedEntry(d, true)));
        plan.AddRange(filesHidden.Select(f => new PlannedEntry(f, false)));
        return plan;
    }

    private static void MoveAll(List<PlannedEntry> plan, string stage, ProgressState ps, CancellationToken ct)
    {
        foreach (var entry in plan)
        {
            ct.ThrowIfCancellationRequested();
            var name = Path.GetFileName(entry.Path);
            TryMove(entry.Path, Path.Combine(stage, name), entry.IsDirectory, ps);
        }
    }

    private static void SweepLeftovers(string current, string stage, ProgressState ps)
    {
        foreach (var leftover in Directory.EnumerateFileSystemEntries(current, "*", SearchOption.TopDirectoryOnly).ToArray())
        {
            if (string.Equals(Path.GetFullPath(leftover), Path.GetFullPath(stage), StringComparison.OrdinalIgnoreCase))
                continue;

            var isDir = Directory.Exists(leftover);
            if (isDir && IsReservedDirectory(leftover))
                continue;

            var name = Path.GetFileName(leftover);
            if (FatDirectoryPacker.IsFiller(name))
                continue;

            TryMove(leftover, Path.Combine(stage, name), isDir, ps);
        }
    }

    /// <summary>
    /// Moves everything in <paramref name="stage"/> back into <paramref name="target"/>, recreating the target
    /// if it was already deleted, and removes the staging folder once it is empty.
    /// </summary>
    /// <returns><see langword="false"/> if anything is left in the staging folder.</returns>
    private static bool TryRestoreStage(string stage, string target)
    {
        try
        {
            if (!Directory.Exists(stage))
                return true;

            Directory.CreateDirectory(target);

            var allMoved = true;
            foreach (var entry in Directory.EnumerateFileSystemEntries(stage).ToArray())
                allMoved &= TryMoveInto(entry, target);

            if (allMoved)
                Directory.Delete(stage, false);

            return allMoved;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static bool TryMoveInto(string entry, string target)
    {
        try
        {
            MoveAny(entry, Path.Combine(target, Path.GetFileName(entry)));
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static bool IsStage(string path) => Path.GetFileName(path).StartsWith(StagePrefix, StringComparison.Ordinal);

    private static bool IsReservedDirectory(string path)
    {
        var name = Path.GetFileName(path);
        return ReservedDirNames.Any(r => StringComparer.OrdinalIgnoreCase.Equals(name, r)) || IsStage(path);
    }

    private static (string[] main, string[] hidden) SplitHidden(string[] paths, bool includeHidden)
    {
        if (includeHidden)
            return (paths, []);

        var main = new List<string>();
        var hidden = new List<string>();

        foreach (var p in paths)
        {
            var name = Path.GetFileName(p);
            var attrs = File.GetAttributes(p);

            var isHiddenAttr = (attrs & (FileAttributes.Hidden | FileAttributes.System)) != 0;
            var isReservedByName = ReservedFileNames.Contains(name, StringComparer.OrdinalIgnoreCase);
            var isReservedByPrefix = ReservedFilePrefixes.Any(pre => name.StartsWith(pre, StringComparison.Ordinal));

            if (isHiddenAttr || isReservedByName || isReservedByPrefix)
                hidden.Add(p);
            else
                main.Add(p);
        }

        return (main.ToArray(), hidden.ToArray());
    }

    private static void TryMove(string src, string dst, bool isDir, ProgressState ps)
    {
        try
        {
            MoveAny(src, dst, isDir);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ps.IncrementErrors();
        }
        finally
        {
            ps.Step();
        }
    }

    private static void MoveAny(string src, string dst, bool? isDirHint = null)
    {
        var isDir = isDirHint ?? Directory.Exists(src);
        if (isDir)
            Directory.Move(src, dst);
        else
            File.Move(src, dst);
    }

    private static void TrySetAttributes(string path, FileAttributes add)
    {
        try
        {
            var a = File.GetAttributes(path);
            File.SetAttributes(path, a | add);
        }
        catch { }
    }

    private static void TryClearAttributes(string path, FileAttributes remove)
    {
        try
        {
            var a = File.GetAttributes(path);
            File.SetAttributes(path, a & ~remove);
        }
        catch { }
    }

    private static void CountWork(string dir, bool rec, bool includeHidden, WorkCounter wc, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var subDirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly)
            .Where(d => !IsReservedDirectory(d)).ToArray();

        if (rec)
        {
            foreach (var sub in subDirs)
            {
                CountWork(sub, true, includeHidden, wc, ct);
            }
        }

        var files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly).Length;

        var isRoot = IsPathRoot(dir);

        wc.Add(files + subDirs.Length);
        wc.Add(isRoot ? files + subDirs.Length + 1 : 2);
    }

    private readonly struct PlannedEntry(string path, bool isDirectory)
    {
        public string Path { get; } = path;

        public bool IsDirectory { get; } = isDirectory;
    }

    private sealed class WorkCounter
    {
        public int Total;

        public void Add(int n)
        {
            checked
            {
                Total += n;
            }
        }
    }

    private sealed class ProgressState
    {
        private readonly Action<double, int>? _cb;
        private readonly int _total;
        private int _done;
        private int _errors;

        public ProgressState(Action<double, int>? cb, int total)
        {
            _cb = cb;
            _total = Math.Max(total, 1);
        }

        public int Errors => _errors;

        public void Step()
        {
            _done++;
            Report();
        }

        public void IncrementErrors() => _errors++;

        public void AddErrors(int count) => _errors += count;

        public void Report(bool forceDone = false) => _cb?.Invoke(forceDone ? 1.0 : Math.Min((double)_done / _total, 0.9999), _errors);
    }
}
