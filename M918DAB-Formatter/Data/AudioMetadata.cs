using FFMpegCore;
using FFMpegCore.Pipes;

namespace M918DAB_Formatter;

public sealed class AudioMetadata
{
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public string? Genre { get; set; }
    public int? TrackNumber { get; set; }
    public int? DiscNumber { get; set; }
    public int? Year { get; set; }
    public TimeSpan Duration { get; set; }
    public Bitmap[] Covers { get; set; } = [];

    public async Task WriteTags(string sourcePath)
    {
        // Build the ffmpeg argument list
        var argBuilder = FFMpegArguments
            .FromFileInput(sourcePath)
            .OutputToFile(
                Path.ChangeExtension(sourcePath, ".tmp"),   // ffmpeg always creates a new file
                overwrite: true,
                options => options
                    .WithCopyCodec() // fast – no re-encode :contentReference[oaicite:0]{index=0}
                    .WithCustomArgument("-id3v2_version 3")   // nicer for MP3 players
                    .WithCustomArgument($"-metadata title={Title}")
                    .WithCustomArgument($"-metadata artist={Artist}")
                    .WithCustomArgument($"-metadata album={Album}")
                    .WithCustomArgument($"-metadata genre={Genre}")
                    .WithCustomArgument($"-metadata track={TrackNumber}")
                    .WithCustomArgument($"-metadata disc={DiscNumber}")
                    .WithCustomArgument($"-metadata date={Year}")
            );

        // Run ffmpeg
        await argBuilder.ProcessAsynchronously();

        // Replace the original atomically
        File.Replace(Path.ChangeExtension(sourcePath, ".tmp"), sourcePath, null);
    }

    public static async Task<AudioMetadata> FromAudioFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            // 1. Run ffprobe --------------------------------------------------
            var analysis = await FFProbe.AnalyseAsync(filePath).ConfigureAwait(false);
            var tags = analysis.Format.Tags ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // helper to read a tag by any of its common names
            string? Tag(params string[] keys) => keys.Select(k => tags.TryGetValue(k, out var v) ? v : null)
                                                     .FirstOrDefault(v => v is not null);

            // 2. Map into our POCO -------------------------------------------
            var meta = new AudioMetadata
            {
                Title = Tag("title", "TIT2"),
                Artist = Tag("artist", "TPE1"),
                Album = Tag("album", "TALB"),
                Genre = Tag("genre", "TCON"),
                TrackNumber = ParseInt(Tag("track", "tracknumber", "TRCK")),
                DiscNumber = ParseInt(Tag("disc", "discnumber", "TPOS")),
                Year = ParseInt(Tag("date", "year", "TYER")),
                Duration = analysis.Format.Duration,
                Covers = await TryExtractCoversAsync(filePath, analysis)
            };

            return meta;
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    private static int? ParseInt(string? s) => int.TryParse(s?.Split('/').FirstOrDefault(), out var x) ? x : null;

    private static async Task<Bitmap[]> TryExtractCoversAsync(string inputPath, IMediaAnalysis analysis)
    {
        var coverCodecs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "mjpeg", "png", "bmp", "webp" };

        var candidates = analysis.VideoStreams
            .Select((v, order) => new { v, order })
            .Where(t => coverCodecs.Contains(t.v.CodecName))
            .ToArray();

        if (candidates.Length == 0)
            return [];

        var covers = new List<Bitmap>(candidates.Length);

        foreach (var candidate in candidates)
        {
            using var ms = new MemoryStream();

            // ffmpeg -i input -map 0:v:<idx> -frames:v 1 -f image2pipe -c copy pipe:1
            await FFMpegArguments
                .FromFileInput(inputPath)
                .OutputToPipe(new StreamPipeSink(ms), o => o
                    .WithCustomArgument($"-map 0:v:{candidate.order}")
                    .WithCustomArgument("-frames:v 1")
                    .WithCustomArgument("-f image2pipe")
                    .WithCustomArgument("-c copy"))
                .ProcessAsynchronously()
                .ConfigureAwait(false);

            ms.Position = 0;
            covers.Add(new Bitmap(ms));
        }

        return [.. covers];
    }
}
