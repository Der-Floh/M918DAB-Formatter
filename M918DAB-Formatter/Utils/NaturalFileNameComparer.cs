using System.Runtime.InteropServices;

namespace M918DAB_Formatter.Utils;

public sealed class NaturalFileNameComparer : IComparer<string>
{
    public static NaturalFileNameComparer Instance { get; } = new();

    private NaturalFileNameComparer()
    {
    }

    public int Compare(string? x, string? y)
    {
        var result = StrCmpLogicalW(x ?? string.Empty, y ?? string.Empty);
        return result != 0 ? result : StringComparer.OrdinalIgnoreCase.Compare(x, y);
    }

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string psz1, string psz2);
}
