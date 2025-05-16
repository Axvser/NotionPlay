using Microsoft.Win32;
using NotionPlay.EditorControls.ViewModels;
using System.IO;

namespace NotionPlay.Tools
{
    public static class FileHelper
    {
        public static readonly string AppRootPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string ProjectsFolder = Path.Combine(AppRootPath, "Projects");
        public static readonly string SnapshotsFolder = Path.Combine(AppRootPath, "Snapshots");
        public static readonly string ConfigsFolder = Path.Combine(AppRootPath, "Configs");

        private static bool CanSaveProjects = true;

        public static string SelectFolder() // 唤起文件夹选择窗口
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
        public static async Task SaveProjectsToDefaultPosition()
        {
            if (CanSaveProjects)
            {
                CanSaveProjects = false;
                if (SourceViewerHost is not null)
                {
                    foreach (var node in SourceViewerHost.TreeNodes)
                    {
                        await TreeItemViewModel.Save(node.Value.ViewModel, ProjectsFolder);
                    }
                }
            }
        } // 仅用于在程序关闭时保存当前所有项目到默认位置
    }
}