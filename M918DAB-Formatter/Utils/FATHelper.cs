namespace M918DAB_Formatter.Utils;

public static class FATHelper
{
    public static IEnumerable<FileSystemInfo> EnumerateInKenwoodOrder(DirectoryInfo dir)
    {
        if (!FatDirectory.IsEntryOrdered(dir.FullName))
            return dir.EnumerateFileSystemInfos();

        if (!FatDirectory.TryRead(dir.FullName, out var entries))
            return dir.EnumerateFileSystemInfos();

        var ordered = new List<FileSystemInfo>(entries.Count);
        foreach (var entry in entries)
        {
            var full = Path.Combine(dir.FullName, entry.Name);
            ordered.Add(entry.IsDirectory ? new DirectoryInfo(full) : new FileInfo(full));
        }

        return ordered;
    }
}
