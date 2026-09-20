namespace M918DAB_Formatter;

public partial class AudioMetadataControl : UserControl
{
    public AudioMetadata? AudioMetadata
    {
        get;
        set
        {
            field = value;
            FillInfosFromAudioMetadata(value);
        }
    }

    public int CurrCoverIndex
    {
        get;
        set
        {
            field = value;
            CurrCoverPictureBox.Image = CurrCover;
        }
    } = -1;

    public Bitmap? CurrCover
    {
        get
        {
            if (AudioMetadata is null || CurrCoverIndex < 0 || CurrCoverIndex >= AudioMetadata.Covers.Length)
                return null;
            return AudioMetadata.Covers[CurrCoverIndex];
        }
    }

    public AudioMetadataControl()
    {
        InitializeComponent();
    }

    private void PrevCoverButton_Click(object sender, EventArgs e)
    {
        CurrCoverIndex = Math.Max(0, CurrCoverIndex - 1);
        PrevCoverButton.Enabled = CurrCoverIndex > 0;
    }

    private void NextCoverButton_Click(object sender, EventArgs e)
    {
        CurrCoverIndex = Math.Min(AudioMetadata?.Covers.Length - 1 ?? 0, CurrCoverIndex + 1);
        NextCoverButton.Enabled = AudioMetadata?.Covers.Length - 1 > CurrCoverIndex;
    }

    private void FillInfosFromAudioMetadata(AudioMetadata? audioMetadata)
    {
        TitleTextBox.Text = audioMetadata?.Title ?? string.Empty;
        ArtistTextBox.Text = audioMetadata?.Artist ?? string.Empty;
        AlbumTextBox.Text = audioMetadata?.Album ?? string.Empty;
        GenreTextBox.Text = audioMetadata?.Genre ?? string.Empty;
        TrackTextBox.Text = audioMetadata?.TrackNumber?.ToString() ?? string.Empty;
        DiscTextBox.Text = audioMetadata?.DiscNumber?.ToString() ?? string.Empty;
        YearTextBox.Text = audioMetadata?.Year?.ToString() ?? string.Empty;

        if (AudioMetadata is not null && AudioMetadata.Covers.Length != 0)
        {
            CurrCoverIndex = 0;
            CoversLabel.Text = $"Cover {CurrCoverIndex + 1} / {AudioMetadata.Covers.Length}";
            NextCoverButton.Enabled = AudioMetadata.Covers.Length - 1 > CurrCoverIndex;
        }
        else
        {
            CurrCoverIndex = -1;
            CoversLabel.Text = $"Cover 0/0";
            NextCoverButton.Enabled = false;
            PrevCoverButton.Enabled = false;
        }
    }
}
