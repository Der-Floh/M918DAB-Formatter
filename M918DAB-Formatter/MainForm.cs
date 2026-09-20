using System.Diagnostics;

using M918DAB_Formatter.Utils;

namespace M918DAB_Formatter;

public sealed partial class MainForm : Form
{
    #region File Sorter
    private CancellationTokenSource? _sortCts;

    public string? CurrSortFolderPath
    {
        get;
        set
        {
            field = value;
            if (string.IsNullOrWhiteSpace(field))
                field = null;

            if (field is not null)
            {
                Properties.Settings.Default.LastSortFolderPath = field;
                Properties.Settings.Default.Save();
            }

            SortFolderPathTextBox.Text = field ?? @"C:\";

            _ = LoadFolderStructure();

            StartSortButton.Enabled = field is not null;
        }
    }

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        CurrSortFolderPath = Properties.Settings.Default.LastSortFolderPath;
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        _sortCts?.Cancel();
        _splitCts?.Cancel();
    }

    private void SelectFolderButton_Click(object sender, EventArgs e)
    {
        CurrSortFolderPath = IOHelper.SelectFolder(Properties.Settings.Default.LastSortFolderPath);
    }

    private async void StartSortButton_Click(object sender, EventArgs e)
    {
        _sortCts = new CancellationTokenSource();
        var token = _sortCts.Token;

        try
        {
            var firstProgress = true;
            StartSortButton.Enabled = false;
            SortProgressBar.Value = 0;
            SortProgressBar.Style = ProgressBarStyle.Marquee;
            SortSuccessLabel.Visible = false;
            SortProgressBar.Value = 0;
            SortProgressLabel.Text = "Preparing...";
            SortProgressLabel.Visible = true;
            var errorCount = await Task.Run(() => SortFileHelper.SortFiles(CurrSortFolderPath, (progressPercent, errorCount) =>
            {
                if (firstProgress)
                {
                    firstProgress = false;
                    if (InvokeRequired)
                        Invoke(new Action(() => SortProgressBar.Style = ProgressBarStyle.Continuous));
                    else
                        SortProgressBar.Style = ProgressBarStyle.Continuous;
                }

                var progressDisplay = progressPercent * 100; // 1 to 100
                progressDisplay *= 100; // 1 to 10000 for last two decimal places
                var roundedProgressDisplay = (int)Math.Round(progressDisplay);

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        SortProgressBar.Value = roundedProgressDisplay;
                        SortProgressLabel.Text = $"{Math.Round(progressPercent * 100, 2):F2}%";
                    }));
                }
                else
                {
                    SortProgressBar.Value = roundedProgressDisplay;
                    SortProgressLabel.Text = $"{Math.Round(progressPercent * 100, 2):F2}%";
                }
            }, token), token);
            SortSuccessLabel.Visible = true;

            CurrSortFolderPath = CurrSortFolderPath;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SortProgressLabel.Visible = false;
            StartSortButton.Enabled = true;
        }
    }

    private void SplitterPathSelectButton_Click(object sender, EventArgs e)
    {
        var filter =
            "Audio Files (*.mp3; *.wav; *.flac; *.aac; *.ogg; *.wma)|" +
            "*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma|" +
            "All Files (*.*)|*.*";
        CurrSplitterPath = IOHelper.SelectFile(filter, Properties.Settings.Default.LastSplitterFolderPath);
    }

    private async Task LoadFolderStructure()
    {
        if (string.IsNullOrWhiteSpace(CurrSortFolderPath) || !Directory.Exists(CurrSortFolderPath))
        {
            LoadFileHelper.LoadFolderStructure(CurrSortFolderPath, SortFilesTreeView);
            return;
        }

        var firstProgress = true;
        LoadingFolderSuccessLabel.Visible = false;
        LoadingFolderProgressBar.Value = 0;
        LoadingFolderProgressBar.Style = ProgressBarStyle.Marquee;
        await Task.Run(() => LoadFileHelper.LoadFolderStructure(CurrSortFolderPath, SortFilesTreeView, 10_000, (progressPercent) =>
        {
            if (firstProgress)
            {
                firstProgress = false;
                if (InvokeRequired)
                    Invoke(new Action(() => LoadingFolderProgressBar.Style = ProgressBarStyle.Continuous));
                else
                    LoadingFolderProgressBar.Style = ProgressBarStyle.Continuous;
            }

            var progressDisplay = progressPercent * 100; // 1 to 100
            progressDisplay *= 100; // 1 to 10000 for last two decimal places
            var roundedProgressDisplay = (int)Math.Round(progressDisplay);

            if (InvokeRequired)
                Invoke(new Action(() => LoadingFolderProgressBar.Value = roundedProgressDisplay));
            else
                LoadingFolderProgressBar.Value = roundedProgressDisplay;
        }));
        LoadingFolderProgressBar.Style = ProgressBarStyle.Continuous;
        LoadingFolderProgressBar.Value = LoadingFolderProgressBar.Maximum;
        LoadingFolderSuccessLabel.Visible = true;
    }

    #endregion

    // ----------------------------------------------------------------
    // Audio Splitter
    // ----------------------------------------------------------------

    #region Audio Splitter

    private CancellationTokenSource? _splitCts;

    public string? CurrSplitterPath
    {
        get;
        set
        {
            field = value;
            if (string.IsNullOrWhiteSpace(field))
                field = null;

            if (field is not null)
            {
                Properties.Settings.Default.LastSplitterFolderPath = Path.GetDirectoryName(field);
                Properties.Settings.Default.Save();
            }

            SplitterPathTextBox.Text = field ?? @"C:\";

            _ = LoadAudioMetadata(field);

            SplitButton.Enabled = field is not null;
        }
    }

    private void SplitTimeTrackBar_Scroll(object sender, EventArgs e)
    {
        SplitTimeNumericUpDown.Value = SplitTimeTrackBar.Value;
        ApplySplitTrackBar();
    }

    private void SplitTimeNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        if (SplitTimeNumericUpDown.Value <= SplitTimeTrackBar.Maximum)
            SplitTimeTrackBar.Value = (int)Math.Round(SplitTimeNumericUpDown.Value);
        ApplySplitTrackBar();
    }

    private async void SplitButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CurrSplitterPath) || !File.Exists(CurrSplitterPath))
        {
            MessageBox.Show("Please select a valid audio file to split.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _splitCts = new CancellationTokenSource();
        var token = _splitCts.Token;

        try
        {
            SplitSuccessLabel.Visible = false;
            SplitProgressBar.Value = 0;
            await Task.Run(() => AudioSplitterHelper.SplitAsync(CurrSplitterPath!, (double)SplitTimeNumericUpDown.Value, (progressPercent) =>
            {
                if (InvokeRequired)
                    BeginInvoke(new Action(() => SplitProgressBar.Value = (int)Math.Round(progressPercent)));
                else
                    SplitProgressBar.Value = (int)Math.Round(progressPercent);
            }, token), token);
            SplitSuccessLabel.Visible = true;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to split audio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (MainTabControl.SelectedIndex == 1 && !FFmpegUtils.IsFFmpegInstalled())
        {
            var result = MessageBox.Show("FFmpeg is needed to split audio files. Do you want to download it now?", "FFmpeg not installed", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (result == DialogResult.Yes)
                Process.Start("https://ffmpeg.org/download.html#build-windows");
        }
    }

    private async Task LoadAudioMetadata(string? path)
    {
        PreviewAudioMetadataControl.AudioMetadata = null;

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return;

        try
        {
            PreviewAudioMetadataControl.AudioMetadata = await AudioMetadata.FromAudioFile(path!);
            SplitPreviewMultiHandleTrackBar.Maximum = (int)Math.Round(PreviewAudioMetadataControl.AudioMetadata.Duration.TotalMinutes);
            SplitPreviewEndLabel.Text = Math.Round(PreviewAudioMetadataControl.AudioMetadata.Duration.TotalMinutes, 2).ToString() + " min";
            ApplySplitTrackBar();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load audio metadata: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplySplitTrackBar()
    {
        if (PreviewAudioMetadataControl.AudioMetadata is null)
            return;

        var duration = PreviewAudioMetadataControl.AudioMetadata.Duration.TotalMilliseconds;
        var splitTime = SplitTimeTrackBar.Value * 1000.0 * 60.0;

        var segments = (int)Math.Floor(duration / splitTime);
        var splitStarts = Enumerable.Range(1, segments).Select(i => (int)Math.Round(i * splitTime / 1000.0 / 60.0)).ToArray();
        SplitPreviewMultiHandleTrackBar.ExtraValues.Clear();
        SplitPreviewMultiHandleTrackBar.ExtraValues.AddRange(splitStarts);
        SplitPreviewMultiHandleTrackBar.Invalidate();

        SplitPreviewLabel.Text = $"Audio Split Preview: {segments + 1} Segments";
    }

    #endregion
}
