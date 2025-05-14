using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotionPlay.VisualComponents
{
    public partial class NumberedMusicalNotationEditor : UserControl, ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            async Task func()
            {
                foreach (Paragraph paragraph in container.Children)
                {
                    if (source.IsCancellationRequested) return;
                    await paragraph.GetSimulation(source).Invoke();
                }
            }
            return (func, source);
        }
        public void AddParagraph(Paragraph paragraph)
        {
            container.Children.Add(paragraph);
        }
        public void RemoveParagraph(Paragraph paragraph)
        {
            container.Children.Remove(paragraph);
        }
        public void Clear()
        {
            container.Children.Clear();
        }
        public void SaveData()
        {
            CanEdit = false;
            foreach (Paragraph paragraph in container.Children)
            {
                paragraph.RunSaved();
            }
            CanEdit = true;
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
    public partial class NumberedMusicalNotationEditor
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
