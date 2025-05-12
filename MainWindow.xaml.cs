global using static NotionPlay.GlobalState;
using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.EditorControls;
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
            var paragraph = new Paragraph() { MusicTheory = Theory };
            paragraph.Items.Add(new Track() { MusicTheory = Theory });
            var paragraphNode = new FileNode()
            {
                MusicTheory = Theory,
                Value = paragraph,
                FileType = FileTypes.Paragraph,
                Height = 20,
                Header = "第一段"
            };
            var projectNode = new FileNode()
            {
                MusicTheory = Theory,
                Value = null,
                FileType = FileTypes.Project,
                Height = 20,
                Header = "晴天"
            };
            projectNode.Items.Add(paragraphNode);
            FileNodes.AddProject(projectNode);
            Editor.AddParagraph(paragraph);
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