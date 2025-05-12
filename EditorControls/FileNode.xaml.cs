using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using System.Windows;

namespace NotionPlay.EditorControls
{
    public partial class FileNode : FileControl
    {
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLocked)
            {
                if (IsOpen)
                {
                    Release();
                    ButtonSymbol = "▶";
                }
                else
                {
                    Redirect();
                    ButtonSymbol = "▼";
                }
            }
        }
        private void Logo_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Title_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public partial class FileNode
    {
        public required IVisualNote Value { get; set; } // 文件节点托管的可视编辑单元

        public string ButtonSymbol
        {
            get { return (string)GetValue(ButtonSymbolProperty); }
            set { SetValue(ButtonSymbolProperty, value); }
        }
        public static readonly DependencyProperty ButtonSymbolProperty =
            DependencyProperty.Register("ButtonSymbol", typeof(string), typeof(FileNode), new PropertyMetadata("▶"));
    }

    [ClickModule]
    [FocusModule]
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    [Hover([nameof(Background), nameof(Foreground)])]
    public partial class FileNode
    {

    }
}
