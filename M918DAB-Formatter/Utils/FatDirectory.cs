using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace M918DAB_Formatter.Utils;

/// <summary>A single entry of a FAT directory table, in on-disk order.</summary>
/// <param name="entryOffset">
/// Byte offset of the entry's short-name record within the parent directory. Any long-name records precede it.
/// </param>
public readonly struct FatDirectoryEntry(string name, uint entryOffset, FileAttributes attributes)
{
    public string Name { get; } = name;

    public uint EntryOffset { get; } = entryOffset;

    public FileAttributes Attributes { get; } = attributes;

    public bool IsDirectory => (Attributes & FileAttributes.Directory) != 0;
}

public static class FatDirectory
{
    public const int DirectoryEntrySize = 32;

    private static readonly string[] EntryOrderedFormats = ["FAT", "FAT32", "EXFAT"];
    private static readonly string[] DeviceReadableFormats = ["FAT", "FAT32"];

    public static bool IsEntryOrdered(string path) => HasFormat(path, EntryOrderedFormats);

    public static bool IsDeviceReadable(string path) => HasFormat(path, DeviceReadableFormats);

    public static string? GetVolumeFormat(string path)
    {
        try
        {
            var root = Path.GetPathRoot(Path.GetFullPath(path));
            return string.IsNullOrEmpty(root) ? null : new DriveInfo(root!).DriveFormat;
        }
        catch
        {
            return null;
        }
    }

    public static bool TryRead(string directoryPath, out IReadOnlyList<FatDirectoryEntry> entries)
    {
        entries = [];
        try
        {
            using var handle = OpenDirectory(directoryPath);
            if (handle is null || handle.IsInvalid)
                return false;

            var collected = new List<FatDirectoryEntry>();
            if (!ReadEntries(handle, collected))
                return false;

            collected.Sort(static (a, b) => a.EntryOffset.CompareTo(b.EntryOffset));
            entries = collected;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool HasFormat(string path, string[] accepted)
    {
        var format = GetVolumeFormat(path)?.ToUpperInvariant();
        return format is not null && accepted.Contains(format);
    }

    private static bool ReadEntries(SafeFileHandle handle, List<FatDirectoryEntry> collected)
    {
        const int BufferSize = 64 * 1024;
        const int ErrorNoMoreFiles = 18;

        var buffer = Marshal.AllocHGlobal(BufferSize);
        try
        {
            if (!GetFileInformationByHandleEx(handle, FileInfoClass.FileFullDirectoryRestartInfo, buffer, BufferSize))
                return false;

            while (true)
            {
                var entry = buffer;
                while (true)
                {
                    var info = Marshal.PtrToStructure<FileFullDirInfo>(entry);
                    var name = ReadName(entry, info.FileNameLength);

                    if (name is not "." and not "..")
                        collected.Add(new FatDirectoryEntry(name, info.FileIndex, (FileAttributes)info.FileAttributes));

                    if (info.NextEntryOffset == 0)
                        break;

                    entry = IntPtr.Add(entry, (int)info.NextEntryOffset);
                }

                if (GetFileInformationByHandleEx(handle, FileInfoClass.FileFullDirectoryInfo, buffer, BufferSize))
                    continue;

                return Marshal.GetLastWin32Error() == ErrorNoMoreFiles;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static string ReadName(IntPtr entry, uint fileNameLength)
    {
        var namePtr = IntPtr.Add(entry, FileNameOffset);
        return Marshal.PtrToStringUni(namePtr, checked((int)fileNameLength) / 2) ?? string.Empty;
    }

    private static readonly int FileNameOffset =
        Marshal.OffsetOf<FileFullDirInfo>(nameof(FileFullDirInfo.FileName)).ToInt32();

    private static SafeFileHandle? OpenDirectory(string path)
    {
        const int FileListDirectory = 0x0001;
        const int FileFlagBackupSemantics = 0x02000000;

        var full = Path.GetFullPath(path);
        var prefixed = full.StartsWith(@"\\", StringComparison.Ordinal) ? full : @"\\?\" + full;

        return CreateFile(prefixed, FileListDirectory, FileShare.Read | FileShare.Write | FileShare.Delete,
            IntPtr.Zero, FileMode.Open, FileFlagBackupSemantics, IntPtr.Zero);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode,
        IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandleEx(SafeFileHandle hFile, FileInfoClass fileInformationClass,
        IntPtr lpFileInformation, uint dwBufferSize);

    private enum FileInfoClass
    {
        FileFullDirectoryInfo = 14,
        FileFullDirectoryRestartInfo = 15
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct FileFullDirInfo
    {
        public uint NextEntryOffset;
        public uint FileIndex;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public long EndOfFile;
        public long AllocationSize;
        public uint FileAttributes;
        public uint FileNameLength;
        public uint EaSize;
        public char FileName;
    }
}
