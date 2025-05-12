using NotionPlay.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NotionPlay.EditorControls
{
    /// <summary>
    /// 节点控件,实现自嵌套的核心组件
    /// </summary>
    public abstract class NodeControl : ItemsControl, IFoldableNode, ILockableNode
    {
        public NodeControl()
        {
            Loaded += (s, e) => RecursiveBuildNodeConnection(this);
        }

        public event Action? ItemsExpanded;
        public event Action? ItemsFolded;
        public event Action? NodeLocked;
        public event Action? NodeUnLocked;

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(NodeControl), new PropertyMetadata(string.Empty));
        public virtual IFoldableNode? ParentNode { get; set; }
        public virtual bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(NodeControl), new PropertyMetadata(false, (dp, e) =>
            {
                if (dp is IFoldableNode note)
                {
                    if ((bool)e.OldValue)
                    {
                        note.OnFolded();
                    }
                    else
                    {
                        note.OnExpanded();
                    }
                }
            }));
        public virtual PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(NodeControl), new PropertyMetadata(PlacementMode.Bottom));
        public virtual bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(NodeControl), new PropertyMetadata(false, (dp, e) =>
            {
                if (dp is IFoldableNode note)
                {
                    if ((bool)e.OldValue)
                    {
                        note.OnLocked();
                    }
                    else
                    {
                        note.OnUnLocked();
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
        public virtual void OnExpanded()
        {
            ItemsExpanded?.Invoke();
        } // 节点展开后
        public virtual void OnFolded()
        {
            ItemsFolded?.Invoke();
        } // 节点折叠后
        public virtual void OnLocked()
        {
            IsOpen = false;
            IsEnabled = false;
            Release();
            NodeLocked?.Invoke();
        } // 节点锁定后
        public virtual void OnUnLocked()
        {
            IsEnabled = true;
            NodeUnLocked?.Invoke();
        } // 节点解锁时

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
