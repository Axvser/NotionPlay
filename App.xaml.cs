using MinimalisticWPF.Theme;
using System.Windows;

namespace NotionPlay
{
    public partial class App : Application
    {
        public App()
        {
            DynamicTheme.FollowSystem(typeof(Dark));
        }
    }
}
