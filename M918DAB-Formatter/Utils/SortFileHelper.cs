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

    public static int SortFiles(string? root, Action<double, int>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(root))
            return 1;

        ReorderByAlphabeticalCopyOrder(root!, true, true, false, true, progressCallback, cancellationToken);
        return 0;
    }

    public static void ReorderByAlphabeticalCopyOrder(string folderPath, bool recursive = false, bool orderDirectoriesToo = false, bool includeHiddenAndSystem = false, bool allowRoot = false, Action<double, int>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentNullException(nameof(folderPath));

        folderPath = Path.GetFullPath(folderPath.Trim().TrimEnd('\\', '/'));
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException(folderPath);

        var root = Path.GetPathRoot(folderPath);
        var isRoot = string.Equals(folderPath.TrimEnd('\\'), (root ?? string.Empty).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);
        if (isRoot && !allowRoot)
            throw new InvalidOperationException("Refusing to reorder the drive root. Set allowRoot:true if you really want this.");

        // Ensure FAT/exFAT
        var drive = new DriveInfo(root);
        var fmt = (drive.DriveFormat ?? string.Empty).ToUpperInvariant();
        if (!fmt.Contains("FAT") && !fmt.Contains("FAT32") && !fmt.Contains("EXFAT"))
            throw new NotSupportedException($"Detected filesystem '{drive.DriveFormat}'. This approach is intended for FAT/FAT32/exFAT.");

        // Count work up front (rough estimate): moves + delete + rename for each directory processed.
        var counter = new WorkCounter();
        CountWork(folderPath, recursive, orderDirectoriesToo, includeHiddenAndSystem, counter, cancellationToken);

        var prog = new ProgressState(progressCallback, counter.Total);
        DoFolder(folderPath, recursive, orderDirectoriesToo, includeHiddenAndSystem, prog, cancellationToken);
        prog.Report(forceDone: true);
    }

    private static void DoFolder(string current, bool rec, bool orderDirs, bool includeHidden, ProgressState ps, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // Depth-first first
        if (rec)
        {
            var dirs = Directory.GetDirectories(current, "*", SearchOption.TopDirectoryOnly).Where(d => !IsReservedOrSkip(d, true, includeHidden));
            foreach (var sub in dirs)
            {
                DoFolder(sub, true, orderDirs, includeHidden, ps, ct);
            }
        }

        var currentRoot = Path.GetPathRoot(current) ?? string.Empty;
        var currentIsRoot = string.Equals(current.TrimEnd('\\'), currentRoot.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);

        var parent = Directory.GetParent(current)?.FullName ?? currentRoot;
        var stage = Path.Combine(parent, ".reorder_stage_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stage);
        TrySetAttributes(stage, FileAttributes.Hidden | FileAttributes.System);

        try
        {
            // Partition
            var filesAll = Directory.GetFiles(current, "*", SearchOption.TopDirectoryOnly);
            var dirsAll = Directory.GetDirectories(current, "*", SearchOption.TopDirectoryOnly).Where(d => !IsReservedOrSkip(d, true, includeHidden)).ToArray();

            var (filesMain, filesHidden) = SplitHidden(filesAll, includeHidden);
            var (dirsMain, dirsHidden) = SplitHidden(dirsAll, includeHidden);

            // Plan = final order
            var plan = new List<(string path, bool isDir)>();
            if (orderDirs)
            {
                var together = new List<(string path, bool isDir)>();
                foreach (var d in dirsMain)
                {
                    together.Add((d, true));
                }
                foreach (var f in filesMain)
                {
                    together.Add((f, false));
                }
                foreach (var e in together.OrderBy(t => Path.GetFileName(t.path), StringComparer.OrdinalIgnoreCase))
                {
                    plan.Add(e);
                }
            }
            else
            {
                foreach (var d in dirsMain)
                {
                    plan.Add((d, true)); // keep dir order
                }
                foreach (var f in filesMain.OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
                {
                    plan.Add((f, false)); // sort files
                }
            }
            foreach (var d in dirsHidden)
            {
                plan.Add((d, true));   // noise at the end
            }
            foreach (var f in filesHidden)
            {
                plan.Add((f, false));
            }

            // Move everything to staging in plan order
            foreach (var (path, isDir) in plan)
            {
                ct.ThrowIfCancellationRequested();
                var name = Path.GetFileName(path);
                var dst = Path.Combine(stage, name);
                TryMove(path, dst, isDir, ps);
            }

            // Also sweep any leftovers, skipping the stage itself & reserved
            foreach (var leftover in Directory.EnumerateFileSystemEntries(current, "*", SearchOption.TopDirectoryOnly).ToArray())
            {
                if (string.Equals(Path.GetFullPath(leftover), Path.GetFullPath(stage), StringComparison.OrdinalIgnoreCase))
                    continue;
                if (Directory.Exists(leftover) && IsReservedOrSkip(leftover, true, includeHidden))
                    continue;

                var name = Path.GetFileName(leftover);
                var dst = Path.Combine(stage, name);
                TryMove(leftover, dst, Directory.Exists(leftover), ps);
            }

            if (currentIsRoot)
            {
                // ROOT-SAFE BRANCH: do NOT delete/rename the root.
                // Move back from staging to root in the SAME final order
                foreach (var (path, isDir) in plan)
                {
                    ct.ThrowIfCancellationRequested();
                    var name = Path.GetFileName(path);
                    var src = Path.Combine(stage, name);
                    var dst = Path.Combine(current, name);
                    TryMove(src, dst, isDir, ps);
                }

                // Move back any leftovers we swept (arbitrary order)
                foreach (var entry in Directory.EnumerateFileSystemEntries(stage).ToArray())
                {
                    var name = Path.GetFileName(entry);
                    var dst = Path.Combine(current, name);
                    TryMove(entry, dst, Directory.Exists(entry), ps);
                }

                TryClearAttributes(stage, FileAttributes.Hidden | FileAttributes.System);
                Directory.Delete(stage, false);
                ps.Step(); // count the delete of stage
            }
            else
            {
                // NON-ROOT: original replace-by-rename approach
                Directory.Delete(current, recursive: false);
                ps.Step();

                TryClearAttributes(stage, FileAttributes.Hidden | FileAttributes.System);
                Directory.Move(stage, current);
                ps.Step();
            }
        }
        catch
        {
            // Best-effort cleanup (unchanged, but skip the stage itself & reserved)
            try
            {
                if (Directory.Exists(stage))
                {
                    foreach (var entry in Directory.EnumerateFileSystemEntries(stage))
                    {
                        var name = Path.GetFileName(entry);
                        var back = Path.Combine(current, name);
                        if (!string.Equals(Path.GetFullPath(entry), Path.GetFullPath(stage), StringComparison.OrdinalIgnoreCase))
                            MoveAny(entry, back);
                    }
                    Directory.Delete(stage, false);
                }
            }
            catch { }
            throw;
        }
    }

    private static bool IsReservedOrSkip(string path, bool isDirectory, bool includeHidden)
    {
        var name = Path.GetFileName(path);
        if (isDirectory && ReservedDirNames.Any(r => StringComparer.OrdinalIgnoreCase.Equals(name, r)))
            return true;

        if (!includeHidden)
        {
            var attrs = File.GetAttributes(path);
            if ((attrs & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                return false; // don't skip; transfer them, just not in the main ordering
        }
        // Always include files; we handle hidden/system partition later
        return false;
    }

    private static (string[] main, string[] hidden) SplitHidden(string[] paths, bool includeHidden)
    {
        if (includeHidden)
            return (paths, new string[0]);

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

    private static void CountWork(string dir, bool rec, bool orderDirs, bool includeHidden, WorkCounter wc, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (rec)
        {
            foreach (var sub in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
            {
                CountWork(sub, true, orderDirs, includeHidden, wc, ct);
            }
        }

        var files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly).Length;
        var dirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly).Length;

        var root = Path.GetPathRoot(dir) ?? string.Empty;
        var isRoot = string.Equals(dir.TrimEnd('\\'), root.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);

        // moves to stage
        wc.Add(files + dirs);

        if (isRoot)
        {
            // plus moves back; no delete+rename
            wc.Add(files + dirs + 1); // +1 for deleting stage dir
        }
        else
        {
            // delete original + rename stage
            wc.Add(2);
        }
    }

    // tiny helpers for progress
    sealed class WorkCounter
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

        public void Step()
        {
            _done++;
            Report();
        }

        public void IncrementErrors() => _errors++;

        public void Report(bool forceDone = false) => _cb?.Invoke(forceDone ? 1.0 : Math.Min((double)_done / _total, 0.9999), _errors);
    }

    private sealed class ProgressContext
    {
        private readonly Action<double, int>? _cb;
        private readonly CancellationToken _ct;
        private long _total;
        private long _done;
        private int _errors;

        public ProgressContext(Action<double, int>? cb, CancellationToken ct)
        {
            _cb = cb;
            _ct = ct;
        }

        public void SetTotal(long total) => _total = Math.Max(0, total);

        public void Step()
        {
            _ct.ThrowIfCancellationRequested();
            _done++;
            Report();
        }

        public void ReportExternalStep()
        {
            _done++;
            Report();
        }

        public void IncrementErrors()
        {
            _errors++;
            Report();
        }

        public void Report(bool forceDone = false)
        {
            if (_cb == null)
                return;
            try
            {
                var total = Math.Max(1, _total);
                var done = forceDone ? total : Math.Min(_done, total);
                var progress = Math.Min(1.0, done / (double)total);
                _cb(progress, _errors);
            }
            catch { }
        }
    }
}
