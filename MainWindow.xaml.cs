global using static NotionPlay.GlobalState;
using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.EditorControls;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents;
using System.Windows;
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
        }

        private void CreateNewEditor(object sender, RoutedEventArgs e)
        {
            Editor.Clear();
            Editor.AddParagraph(new() { MusicTheory = Theory });
            var vma = new TreeItemViewModel()
            {
                Header = "晴天",
                Type = TreeItemTypes.Project,
            };
            var vmb = new TreeItemViewModel()
            {
                Header = "为你翘课的那一天 → 我好想再淋一遍",
                Type = TreeItemTypes.Package,
                Parent = vma,
            };
            var vmc = new TreeItemViewModel()
            {
                Header = "第 21 段",
                Type = TreeItemTypes.Paragraph,
                Parent = vma
            };
            var vmd = new TreeItemViewModel()
            {
                Header = "音轨 1",
                Type = TreeItemTypes.Track,
                Parent = vmb,
            };
            vma.Children.Add(vmb);
            vmb.Children.Add(vmc);
            vmc.Children.Add(vmd);
            var a = new TreeNode(vma);
            SourceManager.AddProject(a);
        }
        private void OpenComponentFile(object sender, RoutedEventArgs e)
        {
            NotificationBox.Confirm("节点事件被触发");
        }
        private void UpdateComponentFile(object sender, RoutedEventArgs e)
        {

        }
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