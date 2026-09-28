namespace M918DAB_Formatter.Utils;

/// <summary>
/// Occupies every free slot in a FAT directory table so that subsequently created entries can only be
/// appended after the existing ones.
/// </summary>
/// <remarks>
/// Windows reuses the slots of deleted directory entries on a first-fit basis. A short 8.3 name needs one
/// 32-byte slot while a long name needs one slot per 13 characters plus one, so a later short name can drop
/// into a small gap ahead of an earlier long name and silently break the on-disk order that FAT-based players
/// use as their playback order. Filling the gaps first removes that possibility; the filler entries are
/// deleted again afterwards, which only leaves unused slots ahead of the ordered ones.
/// </remarks>
public static class FatDirectoryPacker
{
    private const string FillerPrefix = "~PAK";
    private const string FillerExtension = ".TMP";
    private const int FillerNameLength = 12;
    private const int MaxFillers = 8192;
    private const int BatchSize = 8;

    public static bool IsFiller(string name) =>
        name.Length == FillerNameLength
        && name.StartsWith(FillerPrefix, StringComparison.OrdinalIgnoreCase)
        && name.EndsWith(FillerExtension, StringComparison.OrdinalIgnoreCase);

    public static int FillFreeSlots(string directoryPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentNullException(nameof(directoryPath));

        var created = 0;
        while (created < MaxFillers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!FatDirectory.TryRead(directoryPath, out var before) || before.Count == 0)
                break;

            var appendOffset = HighestOffset(before) + FatDirectory.DirectoryEntrySize;
            var batch = CreateBatch(directoryPath, created);
            if (batch.Count == 0)
                break;

            created += batch.Count;

            if (!FatDirectory.TryRead(directoryPath, out var after))
                break;

            if (!AnyBackfilled(after, batch, appendOffset))
                break;
        }

        return created;
    }

    public static int RemoveFillers(string directoryPath)
    {
        var failures = 0;
        foreach (var path in Directory.EnumerateFiles(directoryPath, FillerPrefix + "*" + FillerExtension,
                     SearchOption.TopDirectoryOnly).ToArray())
        {
            if (!IsFiller(Path.GetFileName(path)))
                continue;

            try
            {
                File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                failures++;
            }
        }

        return failures;
    }

    private static bool AnyBackfilled(IReadOnlyList<FatDirectoryEntry> entries, HashSet<string> batch, uint appendOffset)
    {
        foreach (var entry in entries)
        {
            if (entry.EntryOffset < appendOffset && batch.Contains(entry.Name))
                return true;
        }

        return false;
    }

    private static uint HighestOffset(IReadOnlyList<FatDirectoryEntry> entries)
    {
        uint highest = 0;
        foreach (var entry in entries)
        {
            if (entry.EntryOffset > highest)
                highest = entry.EntryOffset;
        }

        return highest;
    }

    private static HashSet<string> CreateBatch(string directoryPath, int startIndex)
    {
        var batch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < BatchSize && startIndex + i < MaxFillers; i++)
        {
            var name = FillerName(startIndex + i);
            try
            {
                using (new FileStream(Path.Combine(directoryPath, name), FileMode.CreateNew, FileAccess.Write))
                {
                }

                batch.Add(name);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // A colliding or unwritable slot simply contributes no filler.
            }
        }

        return batch;
    }

    private static string FillerName(int index) => FillerPrefix + index.ToString("X4") + FillerExtension;
}
