using MinimalisticWPF.Controls;
using NotionPlay.EditorControls;
using NotionPlay.EditorControls.ViewModels;

namespace NotionPlay.Tools
{
    public static class Auxiliary
    {
        public static class TreeItem
        {
            private static TreeItemViewModel? copy;

            public static void Copy(TreeNode source)
            {
                copy = source.ViewModel.DeepCopy();
                NotificationBox.Confirm($"✔ 节点 [ {source.ViewModel.Header} ] 已复制", "成功");
            }
            public static void Paste(TreeNode source)
            {
                if (copy is null) return;

                switch (copy.Type, source.ViewModel.Type)
                {
                    case (TreeItemTypes.Package, TreeItemTypes.Project):
                        var package = Interlocked.Exchange(ref copy, copy.DeepCopy());
                        package.Parent = source.ViewModel;
                        source.ViewModel.Children.Add(package);
                        break;
                    case (TreeItemTypes.Paragraph, TreeItemTypes.Package):
                        var paragraph = Interlocked.Exchange(ref copy, copy.DeepCopy());
                        paragraph.Parent = source.ViewModel;
                        source.ViewModel.Children.Add(paragraph);
                        break;
                    case (TreeItemTypes.Package, TreeItemTypes.Package):
                        var package1 = Interlocked.Exchange(ref copy, copy.DeepCopy());
                        foreach (var item in package1.Children)
                        {
                            item.Parent = source.ViewModel;
                            source.ViewModel.Children.Add(item);
                        }
                        break;
                    case (TreeItemTypes.Paragraph, TreeItemTypes.Paragraph):
                        var paragraph1 = Interlocked.Exchange(ref copy, copy.DeepCopy());
                        source.ViewModel.Notes.Clear();
                        foreach (var item in paragraph1.Notes)
                        {
                            source.ViewModel.Notes.Add(item);
                        }
                        break;
                    default:
                        NotificationBox.Confirm($"❌ 节点 [ {copy.Header} ] 无法粘贴到此处", "失败");
                        break;
                }

                source.ViewModel.UpdateVisual();
            }
        }
    }
}
