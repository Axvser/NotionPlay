using NotionPlay.EditorControls.ViewModels;
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

        private void OpenOrCloseNode(object sender, RoutedEventArgs e)
        {
            // 仅修改ViewModel属性，通过绑定自动更新UI
            ViewModel.IsOpened = !ViewModel.IsOpened;
        }
    }
}
