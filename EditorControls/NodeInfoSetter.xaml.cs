using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using MinimalisticWPF.Tools.String;
using NotionPlay.EditorControls.ViewModels;
using System.Windows;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    public partial class NodeInfoSetter : Window
    {
        public static (string, TreeItemTypes, bool)? Open(TreeItemViewModel viewModel)
        {
            var window = new NodeInfoSetter
            {
                Title = viewModel.Type switch
                {
                    TreeItemTypes.Project => "添加分组",
                    TreeItemTypes.Package => "添加段落",
                    _ => string.Empty
                },
                type = viewModel.Type switch
                {
                    TreeItemTypes.Project => TreeItemTypes.Package,
                    TreeItemTypes.Package => TreeItemTypes.Paragraph,
                    _ => TreeItemTypes.None
                },
                ValueSymbol = viewModel.Type switch
                {
                    TreeItemTypes.Project => "组名 : ",
                    TreeItemTypes.Package => "段落名 : ",
                    _ => string.Empty
                },
            };
            window.inputer.Focus();
            window.ShowDialog();
            if (window.isCancled) return (string.Empty, TreeItemTypes.None, true);
            return (window.HeaderValidator.Validate(window.header) && window.type != TreeItemTypes.None) ? (window.header, window.type, false) : null;
        }
        public static bool Open(SourceViewer sources, out string value)
        {
            var window = new NodeInfoSetter()
            {
                type = TreeItemTypes.Project,
                Title = "添加项目",
                ValueSymbol = "项目名 :",
            };
            window.inputer.Focus();
            window.ShowDialog();
            if (window.HeaderValidator.Validate(window.header) && !window.isCancled && !sources.TreeNodes.ContainsKey(window.header))
            {
                value = window.header;
                return true;
            }
            value = string.Empty;
            return false;
        }
        public static bool Rename(out string value)
        {
            var window = new NodeInfoSetter()
            {
                Title = "重命名",
                ValueSymbol = "新名字 :",
            };
            window.inputer.Focus();
            window.ShowDialog();
            if (window.HeaderValidator.Validate(window.header) && !window.isCancled)
            {
                value = window.header;
                return true;
            }
            value = string.Empty;
            return false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            header = inputer.Text;
        }
        private void Finish(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Cancle(object sender, RoutedEventArgs e)
        {
            isCancled = true;
            Close();
        }
    }

    public partial class NodeInfoSetter
    {
        private bool isCancled = false;
        private string header = string.Empty;
        private TreeItemTypes type = TreeItemTypes.None;

        public string ValueSymbol
        {
            get { return (string)GetValue(ValueSymbolProperty); }
            set { SetValue(ValueSymbolProperty, value); }
        }
        public static readonly DependencyProperty ValueSymbolProperty =
            DependencyProperty.Register("ValueSymbol", typeof(string), typeof(NodeInfoSetter), new PropertyMetadata("输入名称 :"));

        private StringValidator HeaderValidator => new StringValidator()
            .AddRule(s => !string.IsNullOrEmpty(s));
    }
}
