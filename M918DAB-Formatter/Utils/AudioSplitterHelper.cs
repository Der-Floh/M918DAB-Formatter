using System.Globalization;

using FFMpegCore;

namespace M918DAB_Formatter.Utils;

public static class AudioSplitterHelper
{
    public static async Task SplitAsync(string inputPath, double minutes, Action<double> progressCallback, CancellationToken cancellationToken = default)
    {
        var segmentSeconds = minutes * 60.0;
        var segmentSecondsArg = segmentSeconds.ToString("0.###", CultureInfo.InvariantCulture);

        if (segmentSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(segmentSeconds));
        if (!File.Exists(inputPath))
            throw new FileNotFoundException(inputPath);

        var outputDirectory = Path.GetDirectoryName(inputPath);
        var ext = Path.GetExtension(inputPath);
        var outputPattern = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(inputPath)} - Part%03d{ext}");

        var mediaInfo = await FFProbe.AnalyseAsync(inputPath);

        await FFMpegArguments
            .FromFileInput(inputPath)
            .OutputToFile(
                outputPattern,
                overwrite: true,
                options => options
                    .WithCustomArgument("-map 0")                       // keep all streams (e.g. album art)
                    .WithCustomArgument("-f segment")
                    .WithCustomArgument($"-segment_time {segmentSecondsArg}")
                    .WithCustomArgument("-segment_start_number 1")
                    .WithCustomArgument("-reset_timestamps 1")          // each part starts at 0:00
                    .WithCustomArgument("-c copy"))                     // copy, don't transcode
            .CancellableThrough(cancellationToken)
            .NotifyOnProgress(progressCallback, mediaInfo.Duration)
            .ProcessAsynchronously()
            .ConfigureAwait(false);
    }
}
