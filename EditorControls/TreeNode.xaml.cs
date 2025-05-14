using MinimalisticWPF.Controls;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.VisualComponents;
using System.Windows;
using System.Windows.Controls;

namespace NotionPlay.EditorControls
{
    public partial class TreeNode : StackPanel
    {
        public TreeNode()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is TreeItemViewModel viewModel)
                {
                    ViewModel = viewModel;
                    viewModel.UpdateVisual();
                }
            };
        }

        public TreeNode(TreeItemViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
            viewModel.UpdateVisual();
        }

        public TreeItemViewModel ViewModel { get; set; } = TreeItemViewModel.Empty;

        public SourceViewer? SourceViewerHost { get; set; }

        private void OpenOrCloseNode(object sender, RoutedEventArgs e)
        {
            // 仅修改ViewModel属性，通过绑定自动更新UI
            ViewModel.IsOpened = !ViewModel.IsOpened;
        }

        private void Menu_AddChild(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Type == TreeItemTypes.Track)
            {
                NotificationBox.Confirm("⚠ 不可为音轨添加子项", "错误");
                return;
            }
            else
            {
                var meta = NodeInfoSetter.Open(ViewModel);
                if (meta?.Item3 ?? false) return;
                if (meta is null)
                {
                    NotificationBox.Confirm("⚠ 输入了不合法的节点名", "错误");
                    return;
                }
                if (ViewModel.Children.Any(c => c.Header == meta.Value.Item1))
                {
                    NotificationBox.Confirm("⚠ 同一层级存在同名节点", "错误");
                    return;
                }
                var vm = new TreeItemViewModel()
                {
                    Header = meta.Value.Item1,
                    Type = meta.Value.Item2,
                    Parent = ViewModel,
                };
                vm.UpdateVisual();
                ViewModel.Children.Add(vm);
                ViewModel.UpdateVisual();
                ViewModel.IsOpened = true;
            }
        }
        private void Menu_Delete(object sender, RoutedEventArgs e)
        {
            if (NotificationBox.Choose($"⚠ 确定要删除名为 : [ {ViewModel.Header} ] 的节点吗? 此操作不可撤销"))
            {
                ViewModel.Parent.Children.Remove(ViewModel);
                ViewModel.Parent.UpdateVisual();
                if (ViewModel.Type == TreeItemTypes.Project)
                {
                    SourceViewerHost?.RemoveProject(this);
                }
            }
        }
    }
}
