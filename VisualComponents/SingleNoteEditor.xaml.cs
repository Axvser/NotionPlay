using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using System.Windows;

namespace NotionPlay.VisualComponents
{
    public partial class SingleNoteEditor : Window
    {
        private int localIndex;
        private int trackIndex;
        private int paraIndex;
        private SingleNote instance;

        public static void Open(SingleNote note)
        {
            var window = new SingleNoteEditor()
            {
                instance = note,
                localIndex = note.VisualIndex,
                trackIndex = note.ParentNote?.VisualIndex ?? default,
                paraIndex = note.ParentNote?.ParentNote?.VisualIndex ?? default,
            };
            window.UpdateSymbol();
            StopSimulation();
            window.ShowDialog();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseEditor(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public partial class SingleNoteEditor
    {
        public string PositionSymbol
        {
            get { return (string)GetValue(PositionSymbolProperty); }
            set { SetValue(PositionSymbolProperty, value); }
        }
        public static readonly DependencyProperty PositionSymbolProperty =
            DependencyProperty.Register("PositionSymbol", typeof(string), typeof(SingleNoteEditor), new PropertyMetadata(string.Empty));

        public string ValueSymbol
        {
            get { return (string)GetValue(ValueSymbolProperty); }
            set { SetValue(ValueSymbolProperty, value); }
        }
        public static readonly DependencyProperty ValueSymbolProperty =
            DependencyProperty.Register("ValueSymbol", typeof(string), typeof(SingleNoteEditor), new PropertyMetadata(string.Empty));

        public string LevelSymbol
        {
            get { return (string)GetValue(LevelSymbolProperty); }
            set { SetValue(LevelSymbolProperty, value); }
        }
        public static readonly DependencyProperty LevelSymbolProperty =
            DependencyProperty.Register("LevelSymbol", typeof(string), typeof(SingleNoteEditor), new PropertyMetadata(string.Empty));

        public string DurationSymbol
        {
            get { return (string)GetValue(DurationSymbolProperty); }
            set { SetValue(DurationSymbolProperty, value); }
        }
        public static readonly DependencyProperty DurationSymbolProperty =
            DependencyProperty.Register("DurationSymbol", typeof(string), typeof(SingleNoteEditor), new PropertyMetadata(string.Empty));

        private void UpdateSymbol()
        {
            PositionSymbol = $"坐标 : ( {paraIndex + 1} , {trackIndex + 1} , {localIndex + 1} )";
            ValueSymbol = $"音符 : {instance.Note}";
            LevelSymbol = $"音高 : {instance.FrequencyLevel}";
            DurationSymbol = $"持续 : 1 / {(int)instance.DurationType}";
        }

        private void Up_Note(object sender, RoutedEventArgs e) { instance.Up_Note(); UpdateSymbol(); }
        private void Up_Frequency(object sender, RoutedEventArgs e) { instance.Up_Frequency(); UpdateSymbol(); }
        private void Up_Duration(object sender, RoutedEventArgs e) { instance.Up_Duration(); UpdateSymbol(); }
        private void Down_Note(object sender, RoutedEventArgs e) { instance.Down_Note(); UpdateSymbol(); }
        private void Down_Frequency(object sender, RoutedEventArgs e) { instance.Down_Frequency(); UpdateSymbol(); }
        private void Down_Duration(object sender, RoutedEventArgs e) { instance.Down_Duration(); UpdateSymbol(); }
    }

    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(BorderBrush), typeof(Dark), ["White"])]
    [Theme(nameof(BorderBrush), typeof(Light), ["#1e1e1e"])]
    public partial class SingleNoteEditor
    {

    }
}
