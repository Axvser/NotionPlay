using Microsoft.Win32;

namespace NotionPlay.Tools
{
    public static class FileHelper
    {
        public static string RootFile { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;

        public static string SelectFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Multiselect = false
            };
            if (dialog.ShowDialog() ?? false)
            {
                return dialog.FolderName;
            }
            return string.Empty;
        }
    }
}