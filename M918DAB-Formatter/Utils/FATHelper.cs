using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace M918DAB_Formatter.Utils;

public static class FATHelper
{
    public static IEnumerable<FileSystemInfo> EnumerateInKenwoodOrder(DirectoryInfo dir)
    {
        // Detect FAT-like volumes where FileIndex represents directory-entry position.
        var isFatLike = false;
        try
        {
            var root = Path.GetPathRoot(dir.FullName)!;
            var di = new DriveInfo(root);
            var fmt = di.DriveFormat?.ToUpperInvariant();
            isFatLike = fmt is "FAT" or "FAT32" or "EXFAT";
        }
        catch { /* network paths, etc. -> fallback */ }

        if (!isFatLike)
            return dir.EnumerateFileSystemInfos(); // Non-FAT: normal enumeration

        // FAT/exFAT: enumerate via GetFileInformationByHandleEx and sort by FileIndex.
        var list = new List<(string Name, uint Attr, uint FileIndex)>();
        try
        {
            using (var h = CreateDirectoryHandle(dir.FullName))
            {
                if (h is null || h.IsInvalid)
                    return dir.EnumerateFileSystemInfos();

                const int BufferSize = 64 * 1024;
                var buffer = Marshal.AllocHGlobal(BufferSize);
                try
                {
                    // Start (Restart) enumeration
                    if (!GetFileInformationByHandleEx(h, FILE_INFO_BY_HANDLE_CLASS.FileIdExtdDirectoryRestartInfo, buffer, (uint)BufferSize))
                        return dir.EnumerateFileSystemInfos();

                    while (true)
                    {
                        var p = buffer;
                        while (true)
                        {
                            var info = Marshal.PtrToStructure<FILE_ID_EXTD_DIR_INFO>(p);

                            // read FileName (UTF-16) of length FileNameLength bytes
                            var chars = checked((int)info.FileNameLength) / 2;
                            var namePtr = IntPtr.Add(p, Marshal.OffsetOf<FILE_ID_EXTD_DIR_INFO>(nameof(FILE_ID_EXTD_DIR_INFO.FileName)).ToInt32());
                            var name = Marshal.PtrToStringUni(namePtr, chars) ?? string.Empty;

                            if (name is not "." and not "..")
                                list.Add((name, info.FileAttributes, info.FileIndex));

                            if (info.NextEntryOffset == 0)
                                break;

                            p = IntPtr.Add(p, (int)info.NextEntryOffset);
                        }

                        if (!GetFileInformationByHandleEx(h, FILE_INFO_BY_HANDLE_CLASS.FileIdExtdDirectoryInfo, buffer, (uint)BufferSize))
                        {
                            var err = Marshal.GetLastWin32Error();
                            const int ERROR_NO_MORE_FILES = 18;
                            if (err == ERROR_NO_MORE_FILES)
                                break;
                            return dir.EnumerateFileSystemInfos();
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            // On FAT/exFAT, FileIndex is the byte offset in parent directory (i.e., on-disk order).
            // Sort by it to match what the head unit does. :contentReference[oaicite:1]{index=1}
            return [.. list
                .OrderBy(e => e.FileIndex)
                .Select(e =>
                {
                    var full = Path.Combine(dir.FullName, e.Name);
                    var isDir = (e.Attr & (uint)FileAttributes.Directory) != 0;
                    return isDir ? (FileSystemInfo)new DirectoryInfo(full)
                                 : new FileInfo(full);
                })];
        }
        catch
        {
            return dir.EnumerateFileSystemInfos();
        }
    }

    private static SafeFileHandle? CreateDirectoryHandle(string path)
    {
        // Support long paths by adding \\?\ if missing
        var p = path.StartsWith(@"\\?\") ? path : (path.StartsWith(@"\\") ? path : @"\\?\" + path);

        const int FILE_LIST_DIRECTORY = 0x0001;
        const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        const int FILE_ATTRIBUTE_NORMAL = 0x80;

        return CreateFile(p, FILE_LIST_DIRECTORY, FileShare.Read | FileShare.Write | FileShare.Delete, IntPtr.Zero, FileMode.Open, FILE_FLAG_BACKUP_SEMANTICS | FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    private enum FILE_INFO_BY_HANDLE_CLASS
    {
        FileBasicInfo = 0,
        FileStandardInfo = 1,
        FileNameInfo = 2,
        FileRenameInfo = 3,
        FileDispositionInfo = 4,
        FileAllocationInfo = 5,
        FileEndOfFileInfo = 6,
        FileStreamInfo = 7,
        FileCompressionInfo = 8,
        FileAttributeTagInfo = 9,
        FileIdBothDirectoryInfo = 10,          // 0x0A
        FileIdBothDirectoryRestartInfo = 11,   // 0x0B
        FileIoPriorityHintInfo = 12,
        FileRemoteProtocolInfo = 13,
        FileFullDirectoryInfo = 14,
        FileFullDirectoryRestartInfo = 15,
        FileStorageInfo = 16,
        FileAlignmentInfo = 17,
        FileIdInfo = 18,                       // 0x12
        FileIdExtdDirectoryInfo = 19,          // 0x13
        FileIdExtdDirectoryRestartInfo = 20    // 0x14
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FILE_ID_128
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Identifier;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FILE_ID_EXTD_DIR_INFO
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
        public uint ReparsePointTag;
        public FILE_ID_128 FileId;
        public char FileName;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandleEx(SafeFileHandle hFile, FILE_INFO_BY_HANDLE_CLASS FileInformationClass, IntPtr lpFileInformation, uint dwBufferSize);
}
