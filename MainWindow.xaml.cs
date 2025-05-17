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
            LoadConfig();
            AddHotKey();
        }

        private void LoadConfig()
        {
            Loaded += async (s, e) =>
            {
                Settings = await SettingsViewModel.FromFile();
                Theory.Speed = Settings.Speed;
                Theory.LeftNum = Settings.LeftNum;
                Theory.RightNum = Settings.RightNum;
                menu5.Data = $"速度 ⇢ {Settings.Speed}";
                menu6.Data = $"拍号 ⇢ {Settings.LeftNum} / {Settings.RightNum}";
            };
        }
        private void AddHotKey()
        {
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.Z, (s, e) =>
            {
                SubmitSimulation(Editor);
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.X, (s, e) =>
            {
                StopSimulation();
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.C, (s, e) =>
            {
                ChangeRunMode();
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.S, (s, e) =>
            {
                OpenSettings();
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.Plus, (s, e) =>
            {
                var newValue = Math.Clamp(Theory.Speed + 1, 1, int.MaxValue);
                Settings.Speed = newValue;
                Theory.Speed = newValue;
                menu5.Data = $"速度 ⇢ {newValue}";
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.Minus, (s, e) =>
            {
                var newValue = Math.Clamp(Theory.Speed - 1, 1, int.MaxValue);
                Settings.Speed = newValue;
                Theory.Speed = newValue;
                menu5.Data = $"速度 ⇢ {newValue}";
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl, VirtualKeys.Plus, (s, e) =>
            {
                var newValue = Math.Clamp(Theory.LeftNum + 1, 1, int.MaxValue);
                Settings.LeftNum = newValue;
                Theory.LeftNum = newValue;
                menu6.Data = $"拍号 ⇢ {newValue} / {Theory.RightNum}";
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl, VirtualKeys.Minus, (s, e) =>
            {
                var newValue = Math.Clamp(Theory.LeftNum - 1, 1, int.MaxValue);
                Settings.LeftNum = newValue;
                Theory.LeftNum = newValue;
                menu6.Data = $"拍号 ⇢ {newValue} / {Theory.RightNum}";
            });
            GlobalHotKey.Register(VirtualModifiers.Alt, VirtualKeys.Plus, (s, e) =>
            {
                var newValue = Math.Clamp(Theory.RightNum *= 2, 1, 64);
                Settings.RightNum = newValue;
                Theory.RightNum = newValue;
                menu6.Data = $"拍号 ⇢ {Theory.LeftNum} / {newValue}";
            });
            GlobalHotKey.Register(VirtualModifiers.Alt, VirtualKeys.Minus, (s, e) =>
            {
                var newValue = Math.Clamp(Theory.RightNum /= 2, 1, 64);
                Settings.RightNum = newValue;
                Theory.RightNum = newValue;
                menu6.Data = $"拍号 ⇢ {Theory.LeftNum} / {newValue}";
            });
        }

        public void ChangeRunMode()
        {
            StopSimulation();
            var condition = GameMaskVisibility == Visibility.Collapsed;
            GameMaskVisibility = condition ? Visibility.Visible : Visibility.Collapsed;
            CanSimulate = condition;
            CanPreview = !condition;
            CanHightLight = !condition;
            menu1.BeginTransition(condition ? ts_menuFolded : ts_menuExpended);
            menu2.BeginTransition(condition ? ts_buttonExpended : ts_buttonFolded);
            theorysetter.BeginTransition(ts_theorysetterFolded);
        }
        public void OpenSettings()
        {
            if (GameMaskVisibility == Visibility.Visible)
            {
                CanTheorySetter = false;
                theorysetter.BeginTransition(ts_theorysetterFolded);
                return;
            }
            StopSimulation();
            CanTheorySetter = !CanTheorySetter;
            theorysetter.BeginTransition(CanTheorySetter ? ts_theorysetterExpended : ts_theorysetterFolded);
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
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }
        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            if (DynamicTheme.IsThemeChanging) return;
            DynamicTheme.Apply(DynamicTheme.CurrentTheme == typeof(Dark) ? typeof(Light) : typeof(Dark));
        }
        private void OpenHotKeySetter(object sender, RoutedEventArgs e)
        {
            
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
        private static TransitionBoard<StackPanel> ts_theorysetterFolded = Transition.Create<StackPanel>()
            .SetProperty(button => button.Width, 0)
            .SetParams(TransitionParams.Hover);
        private static TransitionBoard<StackPanel> ts_theorysetterExpended = Transition.Create<StackPanel>()
            .SetProperty(button => button.Width, 600)
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
            DependencyProperty.Register("GameMaskVisibility", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(Visibility.Collapsed));

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