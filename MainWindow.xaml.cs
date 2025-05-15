global using static NotionPlay.GlobalState;
using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using MinimalisticWPF.TransitionSystem;
using NotionPlay.EditorControls;
using NotionPlay.EditorControls.ViewModels;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace NotionPlay
{
    public partial class MainWindow : Window
    {
        [Constructor]
        private void InitializeNotes()
        {
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.Z, (s, e) =>
            {
                SubmitSimulation(Editor);
            });
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.X, (s, e) =>
            {
                StopSimulation();
            });
            LocalHotKey.Register(Editor, [Key.LeftCtrl, Key.S], (s, e) =>
            {
                Editor.SaveData();
                NotificationBox.Confirm("√ 已保存");
            });
            EditorHost = Editor;
            SourceViewerHost = SourceManager;
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
            var node = new TreeNode(vm);
            SourceManager.AddProject(node);
        }
        private void OpenOutputWindow(object sender, RoutedEventArgs e)
        {
            StopSimulation();
        }
        private void ChangeRunMode(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            GameMaskVisibility = GameMaskVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            CanSimulate = GameMaskVisibility != Visibility.Collapsed;
            CanPreview = GameMaskVisibility == Visibility.Collapsed;
            CanHightLight = GameMaskVisibility == Visibility.Collapsed;
            menu1.BeginTransition(GameMaskVisibility == Visibility.Collapsed ? ts_buttonExpended : ts_buttonFolded);
            menu2.BeginTransition(GameMaskVisibility == Visibility.Collapsed ? ts_buttonExpended : ts_buttonFolded);
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
        private static TransitionBoard<Button> ts_buttonFolded = Transition.Create<Button>()
            .SetProperty(button => button.Width, 0)
            .SetParams(TransitionParams.Hover);
        private static TransitionBoard<Button> ts_buttonExpended = Transition.Create<Button>()
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
        private void Close_Click(object sender, RoutedEventArgs e)
        {
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