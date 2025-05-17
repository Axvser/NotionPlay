using MinimalisticWPF.Theme;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.EditorControls;
using NotionPlay.Tools;
using System.Windows;

namespace NotionPlay
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DynamicTheme.FollowSystem(typeof(Dark));
            AudioHelper.Initialize();
            Settings = await SettingsViewModel.FromFile();
        }
    }
}
