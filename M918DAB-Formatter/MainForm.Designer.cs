namespace M918DAB_Formatter;

sealed partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.FileSorterTabPage = new System.Windows.Forms.TabPage();
            this.FileSorterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.FileSorterPanel = new System.Windows.Forms.Panel();
            this.SortProgressLabel = new System.Windows.Forms.Label();
            this.LoadingFolderSuccessLabel = new System.Windows.Forms.Label();
            this.LoadingFolderLabel = new System.Windows.Forms.Label();
            this.LoadingFolderProgressBar = new System.Windows.Forms.ProgressBar();
            this.SortProgressBar = new System.Windows.Forms.ProgressBar();
            this.StartSortButton = new System.Windows.Forms.Button();
            this.TargetFolderLabel = new System.Windows.Forms.Label();
            this.SortSuccessLabel = new System.Windows.Forms.Label();
            this.SortFolderPathTextBox = new System.Windows.Forms.TextBox();
            this.SortSelectFolderButton = new System.Windows.Forms.Button();
            this.SortFilesTreeView = new System.Windows.Forms.TreeView();
            this.MusicSplitterTabPage = new System.Windows.Forms.TabPage();
            this.AudioSplitterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AudioSplitterPanel = new System.Windows.Forms.Panel();
            this.AudioFileLabel = new System.Windows.Forms.Label();
            this.SplitSuccessLabel = new System.Windows.Forms.Label();
            this.SplitProgressBar = new System.Windows.Forms.ProgressBar();
            this.SplitterPathTextBox = new System.Windows.Forms.TextBox();
            this.SplitButton = new System.Windows.Forms.Button();
            this.SplitPreviewEndLabel = new System.Windows.Forms.Label();
            this.SplitterPathSelectButton = new System.Windows.Forms.Button();
            this.SplitPreviewStartLabel = new System.Windows.Forms.Label();
            this.SplitTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.SplitPreviewLabel = new System.Windows.Forms.Label();
            this.SplitTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SplitTimeLabel = new System.Windows.Forms.Label();
            this.SplitPreviewMultiHandleTrackBar = new M918DAB_Formatter.MultiHandleTrackBar();
            this.PreviewAudioMetadataControl = new M918DAB_Formatter.AudioMetadataControl();
            this.MainTabControl.SuspendLayout();
            this.FileSorterTabPage.SuspendLayout();
            this.FileSorterTableLayoutPanel.SuspendLayout();
            this.FileSorterPanel.SuspendLayout();
            this.MusicSplitterTabPage.SuspendLayout();
            this.AudioSplitterTableLayoutPanel.SuspendLayout();
            this.AudioSplitterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitPreviewMultiHandleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.FileSorterTabPage);
            this.MainTabControl.Controls.Add(this.MusicSplitterTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(621, 426);
            this.MainTabControl.TabIndex = 0;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            // 
            // FileSorterTabPage
            // 
            this.FileSorterTabPage.Controls.Add(this.FileSorterTableLayoutPanel);
            this.FileSorterTabPage.Location = new System.Drawing.Point(4, 22);
            this.FileSorterTabPage.Name = "FileSorterTabPage";
            this.FileSorterTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.FileSorterTabPage.Size = new System.Drawing.Size(613, 400);
            this.FileSorterTabPage.TabIndex = 0;
            this.FileSorterTabPage.Text = "File Sorter";
            this.FileSorterTabPage.ToolTipText = "Sorts files by changing Creation and Modified dates according to alphabetical sor" +
    "t order.";
            this.FileSorterTabPage.UseVisualStyleBackColor = true;
            // 
            // FileSorterTableLayoutPanel
            // 
            this.FileSorterTableLayoutPanel.ColumnCount = 2;
            this.FileSorterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.76771F));
            this.FileSorterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.23229F));
            this.FileSorterTableLayoutPanel.Controls.Add(this.FileSorterPanel, 0, 0);
            this.FileSorterTableLayoutPanel.Controls.Add(this.SortFilesTreeView, 1, 0);
            this.FileSorterTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileSorterTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.FileSorterTableLayoutPanel.Name = "FileSorterTableLayoutPanel";
            this.FileSorterTableLayoutPanel.RowCount = 1;
            this.FileSorterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FileSorterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 394F));
            this.FileSorterTableLayoutPanel.Size = new System.Drawing.Size(607, 394);
            this.FileSorterTableLayoutPanel.TabIndex = 17;
            // 
            // FileSorterPanel
            // 
            this.FileSorterPanel.Controls.Add(this.SortProgressLabel);
            this.FileSorterPanel.Controls.Add(this.LoadingFolderSuccessLabel);
            this.FileSorterPanel.Controls.Add(this.LoadingFolderLabel);
            this.FileSorterPanel.Controls.Add(this.LoadingFolderProgressBar);
            this.FileSorterPanel.Controls.Add(this.SortProgressBar);
            this.FileSorterPanel.Controls.Add(this.StartSortButton);
            this.FileSorterPanel.Controls.Add(this.TargetFolderLabel);
            this.FileSorterPanel.Controls.Add(this.SortSuccessLabel);
            this.FileSorterPanel.Controls.Add(this.SortFolderPathTextBox);
            this.FileSorterPanel.Controls.Add(this.SortSelectFolderButton);
            this.FileSorterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileSorterPanel.Location = new System.Drawing.Point(3, 3);
            this.FileSorterPanel.Name = "FileSorterPanel";
            this.FileSorterPanel.Size = new System.Drawing.Size(375, 388);
            this.FileSorterPanel.TabIndex = 18;
            // 
            // SortProgressLabel
            // 
            this.SortProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SortProgressLabel.Location = new System.Drawing.Point(135, 349);
            this.SortProgressLabel.Name = "SortProgressLabel";
            this.SortProgressLabel.Size = new System.Drawing.Size(237, 13);
            this.SortProgressLabel.TabIndex = 20;
            this.SortProgressLabel.Text = "Preparing...";
            this.SortProgressLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.SortProgressLabel.Visible = false;
            // 
            // LoadingFolderSuccessLabel
            // 
            this.LoadingFolderSuccessLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadingFolderSuccessLabel.AutoSize = true;
            this.LoadingFolderSuccessLabel.ForeColor = System.Drawing.Color.Green;
            this.LoadingFolderSuccessLabel.Location = new System.Drawing.Point(324, 291);
            this.LoadingFolderSuccessLabel.Name = "LoadingFolderSuccessLabel";
            this.LoadingFolderSuccessLabel.Size = new System.Drawing.Size(48, 13);
            this.LoadingFolderSuccessLabel.TabIndex = 19;
            this.LoadingFolderSuccessLabel.Text = "Success";
            this.LoadingFolderSuccessLabel.Visible = false;
            // 
            // LoadingFolderLabel
            // 
            this.LoadingFolderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LoadingFolderLabel.AutoSize = true;
            this.LoadingFolderLabel.Location = new System.Drawing.Point(3, 291);
            this.LoadingFolderLabel.Name = "LoadingFolderLabel";
            this.LoadingFolderLabel.Size = new System.Drawing.Size(122, 13);
            this.LoadingFolderLabel.TabIndex = 18;
            this.LoadingFolderLabel.Text = "Loading Folder Contents";
            // 
            // LoadingFolderProgressBar
            // 
            this.LoadingFolderProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadingFolderProgressBar.ForeColor = System.Drawing.Color.Green;
            this.LoadingFolderProgressBar.Location = new System.Drawing.Point(3, 307);
            this.LoadingFolderProgressBar.Maximum = 10000;
            this.LoadingFolderProgressBar.Name = "LoadingFolderProgressBar";
            this.LoadingFolderProgressBar.Size = new System.Drawing.Size(369, 23);
            this.LoadingFolderProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.LoadingFolderProgressBar.TabIndex = 17;
            // 
            // SortProgressBar
            // 
            this.SortProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SortProgressBar.ForeColor = System.Drawing.Color.Green;
            this.SortProgressBar.Location = new System.Drawing.Point(84, 365);
            this.SortProgressBar.Maximum = 10000;
            this.SortProgressBar.Name = "SortProgressBar";
            this.SortProgressBar.Size = new System.Drawing.Size(288, 23);
            this.SortProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.SortProgressBar.TabIndex = 3;
            // 
            // StartSortButton
            // 
            this.StartSortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StartSortButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartSortButton.Enabled = false;
            this.StartSortButton.Location = new System.Drawing.Point(3, 365);
            this.StartSortButton.Name = "StartSortButton";
            this.StartSortButton.Size = new System.Drawing.Size(75, 23);
            this.StartSortButton.TabIndex = 4;
            this.StartSortButton.Text = "SORT";
            this.StartSortButton.UseVisualStyleBackColor = true;
            this.StartSortButton.Click += new System.EventHandler(this.StartSortButton_Click);
            // 
            // TargetFolderLabel
            // 
            this.TargetFolderLabel.AutoSize = true;
            this.TargetFolderLabel.Location = new System.Drawing.Point(3, 0);
            this.TargetFolderLabel.Name = "TargetFolderLabel";
            this.TargetFolderLabel.Size = new System.Drawing.Size(70, 13);
            this.TargetFolderLabel.TabIndex = 16;
            this.TargetFolderLabel.Text = "Target Folder";
            // 
            // SortSuccessLabel
            // 
            this.SortSuccessLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SortSuccessLabel.AutoSize = true;
            this.SortSuccessLabel.ForeColor = System.Drawing.Color.Green;
            this.SortSuccessLabel.Location = new System.Drawing.Point(81, 349);
            this.SortSuccessLabel.Name = "SortSuccessLabel";
            this.SortSuccessLabel.Size = new System.Drawing.Size(48, 13);
            this.SortSuccessLabel.TabIndex = 15;
            this.SortSuccessLabel.Text = "Success";
            this.SortSuccessLabel.Visible = false;
            // 
            // SortFolderPathTextBox
            // 
            this.SortFolderPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SortFolderPathTextBox.Location = new System.Drawing.Point(3, 16);
            this.SortFolderPathTextBox.Name = "SortFolderPathTextBox";
            this.SortFolderPathTextBox.ReadOnly = true;
            this.SortFolderPathTextBox.Size = new System.Drawing.Size(288, 20);
            this.SortFolderPathTextBox.TabIndex = 0;
            this.SortFolderPathTextBox.Text = "C:\\";
            // 
            // SortSelectFolderButton
            // 
            this.SortSelectFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SortSelectFolderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SortSelectFolderButton.Location = new System.Drawing.Point(297, 14);
            this.SortSelectFolderButton.Name = "SortSelectFolderButton";
            this.SortSelectFolderButton.Size = new System.Drawing.Size(75, 23);
            this.SortSelectFolderButton.TabIndex = 1;
            this.SortSelectFolderButton.Text = "...";
            this.SortSelectFolderButton.UseVisualStyleBackColor = true;
            this.SortSelectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // SortFilesTreeView
            // 
            this.SortFilesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SortFilesTreeView.Location = new System.Drawing.Point(384, 3);
            this.SortFilesTreeView.Name = "SortFilesTreeView";
            this.SortFilesTreeView.Size = new System.Drawing.Size(220, 388);
            this.SortFilesTreeView.TabIndex = 2;
            // 
            // MusicSplitterTabPage
            // 
            this.MusicSplitterTabPage.Controls.Add(this.AudioSplitterTableLayoutPanel);
            this.MusicSplitterTabPage.Location = new System.Drawing.Point(4, 22);
            this.MusicSplitterTabPage.Name = "MusicSplitterTabPage";
            this.MusicSplitterTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MusicSplitterTabPage.Size = new System.Drawing.Size(613, 400);
            this.MusicSplitterTabPage.TabIndex = 1;
            this.MusicSplitterTabPage.Text = "Audio Splitter";
            this.MusicSplitterTabPage.ToolTipText = "Splits audio files by a specific time amount, like 10min segments for example.";
            this.MusicSplitterTabPage.UseVisualStyleBackColor = true;
            // 
            // AudioSplitterTableLayoutPanel
            // 
            this.AudioSplitterTableLayoutPanel.ColumnCount = 2;
            this.AudioSplitterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.26194F));
            this.AudioSplitterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.73806F));
            this.AudioSplitterTableLayoutPanel.Controls.Add(this.AudioSplitterPanel, 0, 0);
            this.AudioSplitterTableLayoutPanel.Controls.Add(this.PreviewAudioMetadataControl, 1, 0);
            this.AudioSplitterTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AudioSplitterTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.AudioSplitterTableLayoutPanel.Name = "AudioSplitterTableLayoutPanel";
            this.AudioSplitterTableLayoutPanel.RowCount = 1;
            this.AudioSplitterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AudioSplitterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 394F));
            this.AudioSplitterTableLayoutPanel.Size = new System.Drawing.Size(607, 394);
            this.AudioSplitterTableLayoutPanel.TabIndex = 18;
            // 
            // AudioSplitterPanel
            // 
            this.AudioSplitterPanel.Controls.Add(this.AudioFileLabel);
            this.AudioSplitterPanel.Controls.Add(this.SplitSuccessLabel);
            this.AudioSplitterPanel.Controls.Add(this.SplitProgressBar);
            this.AudioSplitterPanel.Controls.Add(this.SplitterPathTextBox);
            this.AudioSplitterPanel.Controls.Add(this.SplitButton);
            this.AudioSplitterPanel.Controls.Add(this.SplitPreviewMultiHandleTrackBar);
            this.AudioSplitterPanel.Controls.Add(this.SplitPreviewEndLabel);
            this.AudioSplitterPanel.Controls.Add(this.SplitterPathSelectButton);
            this.AudioSplitterPanel.Controls.Add(this.SplitPreviewStartLabel);
            this.AudioSplitterPanel.Controls.Add(this.SplitTimeTrackBar);
            this.AudioSplitterPanel.Controls.Add(this.SplitPreviewLabel);
            this.AudioSplitterPanel.Controls.Add(this.SplitTimeNumericUpDown);
            this.AudioSplitterPanel.Controls.Add(this.SplitTimeLabel);
            this.AudioSplitterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AudioSplitterPanel.Location = new System.Drawing.Point(3, 3);
            this.AudioSplitterPanel.Name = "AudioSplitterPanel";
            this.AudioSplitterPanel.Size = new System.Drawing.Size(377, 388);
            this.AudioSplitterPanel.TabIndex = 18;
            // 
            // AudioFileLabel
            // 
            this.AudioFileLabel.AutoSize = true;
            this.AudioFileLabel.Location = new System.Drawing.Point(3, 0);
            this.AudioFileLabel.Name = "AudioFileLabel";
            this.AudioFileLabel.Size = new System.Drawing.Size(53, 13);
            this.AudioFileLabel.TabIndex = 15;
            this.AudioFileLabel.Text = "Audio File";
            // 
            // SplitSuccessLabel
            // 
            this.SplitSuccessLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SplitSuccessLabel.AutoSize = true;
            this.SplitSuccessLabel.ForeColor = System.Drawing.Color.Green;
            this.SplitSuccessLabel.Location = new System.Drawing.Point(81, 349);
            this.SplitSuccessLabel.Name = "SplitSuccessLabel";
            this.SplitSuccessLabel.Size = new System.Drawing.Size(48, 13);
            this.SplitSuccessLabel.TabIndex = 14;
            this.SplitSuccessLabel.Text = "Success";
            this.SplitSuccessLabel.Visible = false;
            // 
            // SplitProgressBar
            // 
            this.SplitProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitProgressBar.ForeColor = System.Drawing.Color.Green;
            this.SplitProgressBar.Location = new System.Drawing.Point(84, 365);
            this.SplitProgressBar.Name = "SplitProgressBar";
            this.SplitProgressBar.Size = new System.Drawing.Size(290, 23);
            this.SplitProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.SplitProgressBar.TabIndex = 13;
            // 
            // SplitterPathTextBox
            // 
            this.SplitterPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitterPathTextBox.Location = new System.Drawing.Point(3, 16);
            this.SplitterPathTextBox.Name = "SplitterPathTextBox";
            this.SplitterPathTextBox.ReadOnly = true;
            this.SplitterPathTextBox.Size = new System.Drawing.Size(290, 20);
            this.SplitterPathTextBox.TabIndex = 1;
            this.SplitterPathTextBox.Text = "C:\\";
            // 
            // SplitButton
            // 
            this.SplitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SplitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SplitButton.Enabled = false;
            this.SplitButton.Location = new System.Drawing.Point(3, 365);
            this.SplitButton.Name = "SplitButton";
            this.SplitButton.Size = new System.Drawing.Size(75, 23);
            this.SplitButton.TabIndex = 12;
            this.SplitButton.Text = "SPLIT";
            this.SplitButton.UseVisualStyleBackColor = true;
            this.SplitButton.Click += new System.EventHandler(this.SplitButton_Click);
            // 
            // SplitPreviewEndLabel
            // 
            this.SplitPreviewEndLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitPreviewEndLabel.Location = new System.Drawing.Point(62, 249);
            this.SplitPreviewEndLabel.Name = "SplitPreviewEndLabel";
            this.SplitPreviewEndLabel.Size = new System.Drawing.Size(312, 13);
            this.SplitPreviewEndLabel.TabIndex = 11;
            this.SplitPreviewEndLabel.Text = "0 min";
            this.SplitPreviewEndLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SplitterPathSelectButton
            // 
            this.SplitterPathSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitterPathSelectButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SplitterPathSelectButton.Location = new System.Drawing.Point(299, 14);
            this.SplitterPathSelectButton.Name = "SplitterPathSelectButton";
            this.SplitterPathSelectButton.Size = new System.Drawing.Size(75, 23);
            this.SplitterPathSelectButton.TabIndex = 2;
            this.SplitterPathSelectButton.Text = "...";
            this.SplitterPathSelectButton.UseVisualStyleBackColor = true;
            this.SplitterPathSelectButton.Click += new System.EventHandler(this.SplitterPathSelectButton_Click);
            // 
            // SplitPreviewStartLabel
            // 
            this.SplitPreviewStartLabel.Location = new System.Drawing.Point(3, 249);
            this.SplitPreviewStartLabel.Name = "SplitPreviewStartLabel";
            this.SplitPreviewStartLabel.Size = new System.Drawing.Size(53, 13);
            this.SplitPreviewStartLabel.TabIndex = 10;
            this.SplitPreviewStartLabel.Text = "0 min";
            // 
            // SplitTimeTrackBar
            // 
            this.SplitTimeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitTimeTrackBar.Location = new System.Drawing.Point(3, 84);
            this.SplitTimeTrackBar.Maximum = 20;
            this.SplitTimeTrackBar.Minimum = 1;
            this.SplitTimeTrackBar.Name = "SplitTimeTrackBar";
            this.SplitTimeTrackBar.Size = new System.Drawing.Size(371, 45);
            this.SplitTimeTrackBar.TabIndex = 5;
            this.SplitTimeTrackBar.Value = 10;
            this.SplitTimeTrackBar.Scroll += new System.EventHandler(this.SplitTimeTrackBar_Scroll);
            // 
            // SplitPreviewLabel
            // 
            this.SplitPreviewLabel.AutoSize = true;
            this.SplitPreviewLabel.Location = new System.Drawing.Point(3, 185);
            this.SplitPreviewLabel.Name = "SplitPreviewLabel";
            this.SplitPreviewLabel.Size = new System.Drawing.Size(160, 13);
            this.SplitPreviewLabel.TabIndex = 9;
            this.SplitPreviewLabel.Text = "Audio Split Preview: 0 Segments";
            // 
            // SplitTimeNumericUpDown
            // 
            this.SplitTimeNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitTimeNumericUpDown.DecimalPlaces = 2;
            this.SplitTimeNumericUpDown.Location = new System.Drawing.Point(299, 135);
            this.SplitTimeNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SplitTimeNumericUpDown.Name = "SplitTimeNumericUpDown";
            this.SplitTimeNumericUpDown.Size = new System.Drawing.Size(75, 20);
            this.SplitTimeNumericUpDown.TabIndex = 7;
            this.SplitTimeNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.SplitTimeNumericUpDown.ValueChanged += new System.EventHandler(this.SplitTimeNumericUpDown_ValueChanged);
            // 
            // SplitTimeLabel
            // 
            this.SplitTimeLabel.AutoSize = true;
            this.SplitTimeLabel.Location = new System.Drawing.Point(3, 68);
            this.SplitTimeLabel.Name = "SplitTimeLabel";
            this.SplitTimeLabel.Size = new System.Drawing.Size(86, 13);
            this.SplitTimeLabel.TabIndex = 8;
            this.SplitTimeLabel.Text = "Split Time in min.";
            // 
            // SplitPreviewMultiHandleTrackBar
            // 
            this.SplitPreviewMultiHandleTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitPreviewMultiHandleTrackBar.Enabled = false;
            this.SplitPreviewMultiHandleTrackBar.Location = new System.Drawing.Point(3, 201);
            this.SplitPreviewMultiHandleTrackBar.Name = "SplitPreviewMultiHandleTrackBar";
            this.SplitPreviewMultiHandleTrackBar.Size = new System.Drawing.Size(371, 45);
            this.SplitPreviewMultiHandleTrackBar.TabIndex = 6;
            this.SplitPreviewMultiHandleTrackBar.TabStop = false;
            // 
            // PreviewAudioMetadataControl
            // 
            this.PreviewAudioMetadataControl.AudioMetadata = null;
            this.PreviewAudioMetadataControl.CurrCoverIndex = -1;
            this.PreviewAudioMetadataControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewAudioMetadataControl.Location = new System.Drawing.Point(386, 3);
            this.PreviewAudioMetadataControl.Name = "PreviewAudioMetadataControl";
            this.PreviewAudioMetadataControl.Size = new System.Drawing.Size(218, 388);
            this.PreviewAudioMetadataControl.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 426);
            this.Controls.Add(this.MainTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(487, 384);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "M-918DAB Formatter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainTabControl.ResumeLayout(false);
            this.FileSorterTabPage.ResumeLayout(false);
            this.FileSorterTableLayoutPanel.ResumeLayout(false);
            this.FileSorterPanel.ResumeLayout(false);
            this.FileSorterPanel.PerformLayout();
            this.MusicSplitterTabPage.ResumeLayout(false);
            this.AudioSplitterTableLayoutPanel.ResumeLayout(false);
            this.AudioSplitterPanel.ResumeLayout(false);
            this.AudioSplitterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitPreviewMultiHandleTrackBar)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl MainTabControl;
    private System.Windows.Forms.TabPage FileSorterTabPage;
    private System.Windows.Forms.TabPage MusicSplitterTabPage;
    private System.Windows.Forms.Button SortSelectFolderButton;
    private System.Windows.Forms.TextBox SortFolderPathTextBox;
    private System.Windows.Forms.TreeView SortFilesTreeView;
    private System.Windows.Forms.Button StartSortButton;
    private System.Windows.Forms.ProgressBar SortProgressBar;
    private System.Windows.Forms.Button SplitterPathSelectButton;
    private System.Windows.Forms.TextBox SplitterPathTextBox;
    private AudioMetadataControl PreviewAudioMetadataControl;
    private System.Windows.Forms.TrackBar SplitTimeTrackBar;
    private MultiHandleTrackBar SplitPreviewMultiHandleTrackBar;
    private System.Windows.Forms.Label SplitTimeLabel;
    private System.Windows.Forms.NumericUpDown SplitTimeNumericUpDown;
    private System.Windows.Forms.Label SplitPreviewLabel;
    private System.Windows.Forms.Label SplitPreviewStartLabel;
    private System.Windows.Forms.Label SplitPreviewEndLabel;
    private System.Windows.Forms.Button SplitButton;
    private System.Windows.Forms.ProgressBar SplitProgressBar;
    private System.Windows.Forms.Label SplitSuccessLabel;
    private System.Windows.Forms.Label SortSuccessLabel;
    private System.Windows.Forms.Label AudioFileLabel;
    private System.Windows.Forms.Label TargetFolderLabel;
    private System.Windows.Forms.TableLayoutPanel FileSorterTableLayoutPanel;
    private System.Windows.Forms.Panel FileSorterPanel;
    private System.Windows.Forms.TableLayoutPanel AudioSplitterTableLayoutPanel;
    private System.Windows.Forms.Panel AudioSplitterPanel;
    private System.Windows.Forms.Label LoadingFolderLabel;
    private System.Windows.Forms.ProgressBar LoadingFolderProgressBar;
    private System.Windows.Forms.Label LoadingFolderSuccessLabel;
    private System.Windows.Forms.Label SortProgressLabel;
}

