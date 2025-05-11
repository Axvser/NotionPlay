using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using System.Windows;

namespace NotionPlay.EditorControls
{
    [ClickModule]
    [Theme(nameof(BorderBrush), typeof(Dark), ["White"])]
    [Theme(nameof(BorderBrush), typeof(Light), ["#1e1e1e"])]
    public partial class MenuNode : MenuControl
    {
        [Constructor]
        private void BuildConnection()
        {
            Loaded += (s, e) => RecursiveBuildNodeConnection(this);
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(MenuNode), new PropertyMetadata(string.Empty));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLocked)
            {
                if (IsOpen)
                {
                    Release();
                }
                else
                {
                    Redirect();
                }
            }
        }

        private void MenuControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsOpen && !IsLocked)
            {
                IsOpen = true;
            }
        }

        private void MenuControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Release();
        }
    }
}
