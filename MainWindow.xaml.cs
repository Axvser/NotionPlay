global using static NotionPlay.GlobalState;
using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.VisualComponents;
using NotionPlay.VisualComponents.Enums;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace NotionPlay
{
    public partial class MainWindow : Window
    {
        private void SingleNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is SingleNote note)
            {
                SingleNoteEditor.Open(note);
            }
        }

        [Constructor]
        private void InitializeNotes()
        {
            var drawer = new Paragraph() { MusicTheory = new() };
            drawer.MusicTheory = new();
            var style = Application.Current.TryFindResource("nicebutton2") as Style;

            var tk1 = new Track() { MusicTheory = new() };
            var s1 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s1.Click += SingleNote_Click;
            var s2 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s2.Click += SingleNote_Click;
            var s3 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s3.Click += SingleNote_Click;
            tk1.Children.Add(s1);
            tk1.Children.Add(s2);
            tk1.Children.Add(s3);

            var tk2 = new Track() { MusicTheory = new() };
            var s4 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s4.Click += SingleNote_Click;
            var s5 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s5.Click += SingleNote_Click;
            var s6 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s6.Click += SingleNote_Click;
            tk2.Children.Add(s4);
            tk2.Children.Add(s5);
            tk2.Children.Add(s6);

            drawer.Children.Add(tk1);
            drawer.Children.Add(tk2);

            Editor.AddParagraph(drawer);

            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.Z, (s, e) =>
            {
                SubmitSimulation(Editor);
            });
        }

        private void MenuNode_Click(object sender, RoutedEventArgs e)
        {
            NotificationBox.Confirm("节点事件被触发");
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