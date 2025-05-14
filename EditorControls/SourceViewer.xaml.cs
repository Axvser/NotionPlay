using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotionPlay.EditorControls
{
    public partial class SourceViewer : UserControl
    {
        public Dictionary<string, TreeNode> TreeNodes { get; set; } = [];
        public void AddProject(TreeNode project)
        {
            if (TreeNodes.ContainsKey(project.ViewModel.Header))
            {
                NotificationBox.Confirm($"⚠ 已存在名为 [ {project.ViewModel.Header} ] 的项目");
                return;
            }
            TreeNodes.Add(project.ViewModel.Header, project);
            container.Children.Add(project);
        }
        public void RemoveProject(TreeNode project)
        {
            if (TreeNodes.Remove(project.ViewModel.Header))
            {
                container.Children.Remove(project);
            }
        }
        public void RemoveProject(string projectName)
        {
            if (TreeNodes.TryGetValue(projectName, out var view))
            {
                container.Children.Remove(view);
            }
        }

        private void Left(object sender, RoutedEventArgs e)
        {
            viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset - viewer.ViewportWidth * 0.2);
        }
        private void Right(object sender, RoutedEventArgs e)
        {
            viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset + viewer.ViewportWidth * 0.2);
        }
        private void Up(object sender, RoutedEventArgs e)
        {
            viewer.ScrollToVerticalOffset(viewer.VerticalOffset + viewer.ViewportHeight * 0.2);
        }
        private void Down(object sender, RoutedEventArgs e)
        {
            viewer.ScrollToVerticalOffset(viewer.VerticalOffset - viewer.ViewportHeight * 0.2);
        }

        private void Viewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                {
                    scale.ScaleX = Math.Clamp(scale.ScaleX + 0.1, 1, 5);
                    scale.ScaleY = Math.Clamp(scale.ScaleY + 0.1, 1, 5);
                }
                else
                {
                    scale.ScaleX = Math.Clamp(scale.ScaleX - 0.1, 1, 5);
                    scale.ScaleY = Math.Clamp(scale.ScaleY - 0.1, 1, 5);
                }
            }
        }
    }

    [FocusModule]
    [Theme(nameof(Background), typeof(Dark), ["default"])]
    [Theme(nameof(Background), typeof(Light), ["default"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    public partial class SourceViewer
    {
        private readonly ScaleTransform scale = new();

        [Constructor]
        private void SetContentScale()
        {
            container.LayoutTransform = scale;
            LocalHotKey.Register(this, [Key.OemPlus], (s, e) =>
            {
                scale.ScaleX = Math.Clamp(scale.ScaleX + 0.1, 1, 5);
                scale.ScaleY = Math.Clamp(scale.ScaleY + 0.1, 1, 5);
            });
            LocalHotKey.Register(this, [Key.OemMinus], (s, e) =>
            {
                scale.ScaleX = Math.Clamp(scale.ScaleX - 0.1, 1, 5);
                scale.ScaleY = Math.Clamp(scale.ScaleY - 0.1, 1, 5);
            });
        }
    }
}
