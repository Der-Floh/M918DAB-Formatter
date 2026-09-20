namespace M918DAB_Formatter.Utils;

public static class IOHelper
{
    public static string? SelectFolder(string? selectedPath = null)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select a folder"
        };

        if (!string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath))
            dialog.SelectedPath = selectedPath;

        var result = dialog.ShowDialog();

        if (result == DialogResult.OK)
            return dialog.SelectedPath;
        else
            return null;
    }

    public static string? SelectFile(string filter, string? selectedPath = null)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Select a music file",
            Filter = filter,
            CheckFileExists = true,
            Multiselect = false,
        };

        if (!string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath))
            dialog.InitialDirectory = selectedPath;
        else
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        if (dialog.ShowDialog() == DialogResult.OK)
            return dialog.FileName;
        else
            return null;
    }
}
