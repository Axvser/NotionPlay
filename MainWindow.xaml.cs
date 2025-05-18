global using static NotionPlay.GlobalState;
using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using MinimalisticWPF.TransitionSystem;
using NotionPlay.EditorControls;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace NotionPlay
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }

        [Constructor]
        private void InitializeNotes()
        {
            Instance = this;
            EditorHost = Editor;
            SourceViewerHost = SourceManager;
            Loaded += (s, e) =>
            {
                GameVisual.Instance.Show();
                GameVisual.Instance.Visibility = Visibility.Hidden;
                GameVisual.Instance.Opacity = 1;
            };
            LoadConfig();
        }

        private void LoadConfig()
        {
            Theory.Speed = Settings.Speed;
            Theory.LeftNum = Settings.LeftNum;
            Theory.RightNum = Settings.RightNum;
            Loaded += (s, e) => UpdateTheoryText();
        }
        public void UpdateTheoryText()
        {
            menu5.Data = $"速度 ⇢ {Settings.Speed}";
            menu6.Data = $"拍号 ⇢ {Settings.LeftNum} / {Settings.RightNum}";
        }

        public void ChangeRunMode()
        {
            StopSimulation();
            var condition = GameMaskVisibility == Visibility.Hidden;
            GameMaskVisibility = condition ? Visibility.Visible : Visibility.Hidden;
            CanSimulate = condition;
            CanPreview = !condition;
            CanHightLight = !condition;
            CanTheorySetter = condition;
            LoadingAnimator.CanMonoBehaviour = condition;
            menu1.BeginTransition(condition ? ts_menuFolded : ts_menuExpended);
            menu2.BeginTransition(condition ? ts_buttonExpended : ts_buttonFolded);
        }

        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            if (NodeInfoSetter.NewProject(out var value))
            {
                var vma = new TreeItemViewModel()
                {
                    Header = value,
                    Type = TreeItemTypes.Project,
                };
                var a = new TreeNode(vma);
                SourceManager.AddProject(a);
            }
        }
        private async void OpenComponentFile(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            var vm = await TreeItemViewModel.FromFile();
            if (vm == TreeItemViewModel.Empty) return;
            SourceManager.RemoveProject(vm.Header);
            Editor.Clear();
            var node = new TreeNode(vm);
            SourceManager.AddProject(node);
        }
        private void OpenOutputWindow(object sender, RoutedEventArgs e)
        {
            StopSimulation();
        }
        private void ChangeRunMode(object sender, RoutedEventArgs e)
        {
            ChangeRunMode();
        }
        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            if (DynamicTheme.IsThemeChanging) return;
            DynamicTheme.Apply(DynamicTheme.CurrentTheme == typeof(Dark) ? typeof(Light) : typeof(Dark));
        }
        private void OpenHotKeySetter(object sender, RoutedEventArgs e)
        {
            HotKeySetter.Instance.ShowDialog();
        }
        private async void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            EditorHost?.SaveData();
            await FileHelper.SaveProjectsToDefaultPosition();
            NotificationBox.Confirm("✔ 所有项目已保存", "成功");
        }
        private async void SelectSnapshots(object sender, RoutedEventArgs e)
        {
            await GameVisual.Instance.LoadMultipleSnapshots();
        }
    }

    public partial class MainWindow
    {
        private static TransitionBoard<MenuNode> ts_menuFolded = Transition.Create<MenuNode>()
            .SetProperty(menu => menu.Width, 0)
            .SetParams(TransitionParams.Hover);
        private static TransitionBoard<MenuNode> ts_menuExpended = Transition.Create<MenuNode>()
            .SetProperty(menu => menu.Width, 100)
            .SetParams(TransitionParams.Hover);
        private static TransitionBoard<MinimalisticWPF.Controls.Button> ts_buttonFolded = Transition.Create<MinimalisticWPF.Controls.Button>()
            .SetProperty(button => button.Width, 0)
            .SetParams(TransitionParams.Hover);
        private static TransitionBoard<MinimalisticWPF.Controls.Button> ts_buttonExpended = Transition.Create<MinimalisticWPF.Controls.Button>()
            .SetProperty(button => button.Width, 100)
            .SetParams(TransitionParams.Hover);
    }

    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    [Theme(nameof(BorderBrush), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(BorderBrush), typeof(Dark), ["White"])]
    public partial class MainWindow
    {
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            var isMax = WindowState == WindowState.Maximized;
            bt_close.Data = (Application.Current.TryFindResource(isMax ? "SVG_WinUnMax" : "SVG_WinMax") as Path)?.Data.ToString() ?? string.Empty;
            CornerRadius = isMax ? new CornerRadius(0) : new CornerRadius(10d);
            LAR.Height = isMax ? new GridLength(600) : new GridLength(300);
            LAC.Width = isMax ? new GridLength(600) : new GridLength(300);
        }
        protected async override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Settings = await SettingsViewModel.FromFile();
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MainWindow), new PropertyMetadata(new CornerRadius(10d)));

        public Visibility GameMaskVisibility
        {
            get { return (Visibility)GetValue(GameMaskVisibilityProperty); }
            set { SetValue(GameMaskVisibilityProperty, value); }
        }
        public static readonly DependencyProperty GameMaskVisibilityProperty =
            DependencyProperty.Register("GameMaskVisibility", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(Visibility.Hidden));

        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await FileHelper.SaveProjectsToDefaultPosition();
            await SettingsViewModel.SaveFile(Settings);
            await GameVisualViewModel.SaveFile((GameVisualViewModel)GameVisual.Instance.DataContext);
            HotKeySetter.Instance.Close();
            GameVisual.Instance.Close();
            Close();
        }
        private void Size_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        private void Minim_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}