using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using System.Windows;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["#99FFFFFF"])]
    [Theme(nameof(Background), typeof(Dark), ["#AA1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["Black"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    public partial class GameVisual : Window
    {
        public static GameVisual Instance { get; set; } = new();

        public static void ChangeState()
        {
            Instance.Visibility = Instance.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }   

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GameVisual), new PropertyMetadata(new CornerRadius(10)));
    }
}
