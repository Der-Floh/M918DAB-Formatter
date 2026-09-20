using FFMpegCore;
using FFMpegCore.Helpers;

namespace M918DAB_Formatter.Utils;

public static class FFmpegUtils
{
    public static bool IsFFmpegInstalled()
    {
        try
        {
            FFMpegHelper.VerifyFFMpegExists(new FFOptions());
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
