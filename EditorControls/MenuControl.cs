using NotionPlay.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NotionPlay.EditorControls
{
    public partial class MenuControl : ItemsControl, IFoldableNode, ILockableNode
    {
        public IFoldableNode? ParentNode { get; set; }
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(MenuControl), new PropertyMetadata(false));
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(MenuControl), new PropertyMetadata(PlacementMode.Right));
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(MenuControl), new PropertyMetadata(false, (dp, e) =>
            {
                if (dp is IFoldableNode note)
                {
                    note.IsOpen = (bool)e.OldValue;
                    note.IsEnabled = (bool)e.OldValue;
                    if (!(bool)e.OldValue)
                    {
                        note.Release();
                    }
                }
            }));

        public virtual void Redirect()
        {
            if (ParentNode is not null)
            {
                foreach (IFoldableNode node in ParentNode.Items)
                {
                    RecursiveRelease(node);
                }
            }
            IsOpen = true;
        } // 重定向至当前节点
        public virtual void Release()
        {
            RecursiveRelease(this);
        } // 释放此节点

        protected static void RecursiveBuildNodeConnection(IFoldableNode startNode) // 递归构建Node之间的父子关系
        {
            var targets = startNode.Items.OfType<IFoldableNode>();
            if (!targets.Any()) return;
            foreach (var target in targets)
            {
                target.ParentNode = startNode;
                RecursiveBuildNodeConnection(target);
            }
        }
        protected static void RecursiveRelease(IFoldableNode? startNode)
        {
            if (startNode is null) return;
            startNode.IsOpen = false;
            foreach (var item in startNode.Items) // Items 可能存在非折叠节点
            {
                if (item is IFoldableNode foldable)
                {
                    foldable.IsOpen = false;
                    RecursiveRelease(foldable);
                }
            }
        } // 递归释放从指定节点开始的所有节点
    }
}
