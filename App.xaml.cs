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
            Settings = await SettingsViewModel.FromFile();
            DynamicTheme.ThemeChangeLoaded += (s, e) =>
            {
                Settings.IsDark = DynamicTheme.CurrentTheme == typeof(Dark);
            };
            DynamicTheme.FollowSystem(Settings.IsDark ? typeof(Dark) : typeof(Light));
            AudioHelper.Initialize();
            HotKeySetter.Instance.DataContext = Settings;
        }
    }
}
