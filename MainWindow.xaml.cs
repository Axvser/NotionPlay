global using static NotionPlay.GlobalState;
using MinimalisticWPF.Controls;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using MinimalisticWPF.TransitionSystem;
using NotionPlay.EditorControls;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NotionPlay
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public static void UpdateGameVisualText(string text)
        {
            if (Instance is not null)
            {
                Instance.GamePopup.TaskControlSymbol = text;
            }
        }
        public static void ChangeGameVisualState()
        {
            Instance?.GamePopup.ChangeState();
        }
        public static void StartLoadingTransition()
        {
            if (Instance is null) return;
            Instance.ProgressBackground = Brushes.Violet;
        }
        public static void EndLoadingTransition()
        {
            if(Instance is null) return;
            Instance.ProgressBackground = Brushes.White;
        }

        [Constructor]
        private void InitializeNotes()
        {
            Instance = this;
            EditorHost = Editor;
            GameVisualHost = GamePopup;
            SourceViewerHost = SourceManager;
            SourceInitialized += async (s, e) =>
            {
                var gameVisualVM = await GameVisualViewModel.FromFile();
                GamePopup.DataContext = gameVisualVM;
                GamePopup.ViewModel = gameVisualVM;
            };
            Loaded += (s, e) =>
            {
                Theory.Speed = Settings.Speed;
                Theory.LeftNum = Settings.LeftNum;
                Theory.RightNum = Settings.RightNum;
                UpdateTheoryText();
            };
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
            if (CanPreview) GamePopup.ChangeState();
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
            StopSimulation();
            HotKeySetter.Instance.ShowDialog();
        }
        private async void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            EditorHost?.SaveData();
            await FileHelper.SaveProjectsToDefaultPosition();
            NotificationBox.Confirm("✔ 所有项目已保存", "成功");
        }
        private async void SelectSnapshots(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            await GamePopup.LoadSingleSnapshotAsync();
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
        protected override async void OnClosing(CancelEventArgs e)
        {
            if (!isSavedWhileClose)
            {
                await FileHelper.SaveProjectsToDefaultPosition();
                await SettingsViewModel.SaveFile(Settings);
                await GameVisualViewModel.SaveFile(GamePopup.ViewModel);
                HotKeySetter.Instance.Close();
                GamePopup.IsOpen = false;
            }
            base.OnClosing(e);
        }
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

        public Brush ProgressBackground
        {
            get { return (Brush)GetValue(ProgressBackgroundProperty); }
            set { SetValue(ProgressBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ProgressBackgroundProperty =
            DependencyProperty.Register("ProgressBackground", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.White));

        public Transform ProgressTransform
        {
            get { return (Transform)GetValue(ProgressTransformProperty); }
            set { SetValue(ProgressTransformProperty, value); }
        }
        public static readonly DependencyProperty ProgressTransformProperty =
            DependencyProperty.Register("ProgressTransform", typeof(Transform), typeof(MainWindow), new PropertyMetadata(Transform.Identity));

        public Visibility GameMaskVisibility
        {
            get { return (Visibility)GetValue(GameMaskVisibilityProperty); }
            set { SetValue(GameMaskVisibilityProperty, value); }
        }
        public static readonly DependencyProperty GameMaskVisibilityProperty =
            DependencyProperty.Register("GameMaskVisibility", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(Visibility.Hidden));

        public bool isSavedWhileClose = false;
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
            await GameVisualViewModel.SaveFile(GamePopup.ViewModel);
            HotKeySetter.Instance.Close();
            GamePopup.IsOpen = false;
            isSavedWhileClose = true;
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