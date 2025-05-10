global using static NotionPlay.GlobalState;
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
            Drawer.MusicTheory = new();
            var style = Application.Current.TryFindResource("nicebutton2") as Style;
            var s1 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s1.Click += SingleNote_Click;
            var s2 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s2.Click += SingleNote_Click;
            var s3 = new SingleNote(Notes.Do, DurationTypes.Four, FrequencyLevels.High) { MusicTheory = new(), Width = 15, Height = 50, Style = style };
            s3.Click += SingleNote_Click;
            Drawer.Children.Add(s1);
            Drawer.Children.Add(s2);
            Drawer.Children.Add(s3);
            Drawer.UpdateVisualMeta();

            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Shift, VirtualKeys.Z, (s, e) =>
            {
                SubmitSimulation(Drawer);
            });
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