using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NotionPlay.EditorControls
{
    [ClickModule]
    [Theme(nameof(BorderBrush), typeof(Dark), ["White"])]
    [Theme(nameof(BorderBrush), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Background), typeof(Light), ["White"])]
    public partial class MenuNode : ItemsControl, IFoldableNode, ILockableNode
    {
        [Constructor]
        private void BuildConnection()
        {
            Loaded += (s, e) => RecursiveBuildNodeConnection(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLocked)
            {
                if (IsOpen)
                {
                    if (Items.Count > 0)
                    {
                        Release();
                    }
                    else
                    {
                        var root = FindRoot(ParentNode);
                        if (root is not null) root.IsOpen = false;
                    }
                }
                else
                {
                    Redirect();
                }
            }
        }
        private void MenuControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsOpen && !IsLocked && ParentNode is not null)
            {
                IsOpen = true;
            }
        }
        private void MenuControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Release();
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
            DependencyProperty.Register("Header", typeof(string), typeof(MenuNode), new PropertyMetadata(string.Empty));
        public virtual IFoldableNode? ParentNode { get; set; }
        public virtual bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(MenuNode), new PropertyMetadata(false, (dp, e) =>
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
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(MenuNode), new PropertyMetadata(PlacementMode.Right));
        public virtual bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(MenuNode), new PropertyMetadata(false, (dp, e) =>
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
            BorderThickness = Items.Count > 0 ? new Thickness(1) : new Thickness(0);
        } // 节点展开后
        public virtual void OnFolded()
        {
            ItemsFolded?.Invoke();
            BorderThickness = new Thickness(0);
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

        protected static void RecursiveBuildNodeConnection(IFoldableNode startNode, Action<IFoldableNode>? callback = null) // 递归构建Node之间的父子关系
        {
            var targets = startNode.Items.OfType<IFoldableNode>();
            if (!targets.Any()) return;
            foreach (var target in targets)
            {
                target.ParentNode = startNode;
                RecursiveBuildNodeConnection(target);
                callback?.Invoke(target);
            }
        }
        protected static void RecursiveRelease(IFoldableNode? startNode, Action<IFoldableNode>? callback = null)
        {
            if (startNode is null) return;
            startNode.IsOpen = false;
            foreach (var item in startNode.Items)
            {
                if (item is IFoldableNode foldable)
                {
                    foldable.IsOpen = false;
                    RecursiveRelease(foldable);
                    callback?.Invoke(foldable);
                }
            }
        } // 递归释放从指定节点开始的所有节点
        private static IFoldableNode? FindRoot(IFoldableNode? startNode)
        {
            if (startNode?.ParentNode is null) return startNode;
            return FindRoot(startNode.ParentNode);
        }
    }
}
