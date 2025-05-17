using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using System.Windows;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    public partial class HotKeySetter : Window
    {
        public static HotKeySetter Instance { get; set; } = new();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Instance.Hide();
        }
        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
