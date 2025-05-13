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
        }

        public void AddNode(TreeNode node)
        {
            ViewModel.Children.Add(node.ViewModel);
        }
        public void RemoveNode(TreeNode node)
        {
            ViewModel.Children.Remove(node.ViewModel);
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(TreeNode), new PropertyMetadata(string.Empty, (dp, e) =>
            {
                if (dp is TreeNode node)
                {
                    node.ViewModel.Header = (string)e.NewValue;
                }
            }));

        public TreeItemTypes ItemType
        {
            get { return (TreeItemTypes)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(TreeItemTypes), typeof(TreeNode), new PropertyMetadata(TreeItemTypes.None, (dp, e) =>
            {
                if (dp is TreeNode node)
                {
                    node.ViewModel.Type = (TreeItemTypes)e.NewValue;
                }
            }));

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(TreeNode), new PropertyMetadata(20d));
        public string StateSymbol
        {
            get { return (string)GetValue(StateSymbolProperty); }
            set { SetValue(StateSymbolProperty, value); }
        }
        public static readonly DependencyProperty StateSymbolProperty =
            DependencyProperty.Register("StateSymbol", typeof(string), typeof(TreeNode), new PropertyMetadata(string.Empty));
        public string TypeSymbol
        {
            get { return (string)GetValue(TypeSymbolProperty); }
            set { SetValue(TypeSymbolProperty, value); }
        }
        public static readonly DependencyProperty TypeSymbolProperty =
            DependencyProperty.Register("TypeSymbol", typeof(string), typeof(TreeNode), new PropertyMetadata(string.Empty));

        private void OpenOrCloseNode(object sender, RoutedEventArgs e)
        {
            if (ItemsPanel.Visibility == Visibility.Visible)
            {
                ItemsPanel.Visibility = Visibility.Collapsed;
                ViewModel.IsOpened = false;
            }
            else
            {
                ItemsPanel.Visibility = Visibility.Visible;
                ViewModel.IsOpened = true;
            }
        }
    }
}
