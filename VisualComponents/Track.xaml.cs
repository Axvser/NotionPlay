using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WindowsInput;
using WindowsInput.Native;

namespace NotionPlay.VisualComponents
{
    public partial class Track : ItemsControl, IVisualNote, ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            async Task func()
            {
                List<(VirtualKeyCode, int, Action, Action)> notes = [];
                foreach (var item in Items)
                {
                    if (item is SingleNote note && KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var key))
                    {
                        notes.Add((key, MusicTheory.GetSpan(note.DurationType),
                            () =>
                            {
                                note.Background = note.SimulatingBrush;
                            },
                            () =>
                            {
                                note.Background = note.CurrentTheme == typeof(Dark) ? note.DarkBackground : note.LightBackground;
                            }
                        ));
                    }
                }
                try
                {
                    foreach (var note in notes)
                    {
                        Simulator.Keyboard.KeyDown(note.Item1);
                        note.Item3.Invoke();
                        await Task.Delay(note.Item2, source.Token);
                        Simulator.Keyboard.KeyUp(note.Item1);
                        note.Item4.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    foreach (var note in notes)
                    {
                        Simulator.Keyboard.KeyUp(note.Item1);
                        note.Item4.Invoke();
                    }
                }
            }
            return (func, source);
        }

        private void Bottom_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BottomBorderVisibility = Visibility.Visible;
        }
        private void Bottom_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BottomBorderVisibility = Visibility.Collapsed;
        }
        private void Top_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TopBorderVisibility = Visibility.Visible;
        }
        private void Top_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TopBorderVisibility = Visibility.Collapsed;
        }

        private void option1_Click(object sender, RoutedEventArgs e)
        {
            var note = new SingleNote(Notes.None, DurationTypes.Sixteen, FrequencyLevels.Middle) { MusicTheory = MusicTheory, Width = 15, Height = 50, Style = default_notestyle };
            Items.Add(note);
        }

        private void option2_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        }
    }

    [Theme(nameof(Background), typeof(Dark), ["Transparent"])]
    [Theme(nameof(Background), typeof(Light), ["Transparent"])]
    [Hover([nameof(Background)])]
    public partial class Track
    {
        private readonly static Style? default_notestyle = Application.Current.TryFindResource("nicebutton2") as Style;
        public InputSimulator Simulator { get; } = new();
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; } = 0;
        public VisualTypes VisualType { get; set; } = VisualTypes.Track;

        internal Visibility BottomBorderVisibility
        {
            get { return (Visibility)GetValue(BottomBorderVisibilityProperty); }
            set { SetValue(BottomBorderVisibilityProperty, value); }
        }
        internal static readonly DependencyProperty BottomBorderVisibilityProperty =
            DependencyProperty.Register("BottomBorderVisibility", typeof(Visibility), typeof(Track), new PropertyMetadata(Visibility.Collapsed));

        internal Visibility TopBorderVisibility
        {
            get { return (Visibility)GetValue(TopBorderVisibilityProperty); }
            set { SetValue(TopBorderVisibilityProperty, value); }
        }
        internal static readonly DependencyProperty TopBorderVisibilityProperty =
            DependencyProperty.Register("TopBorderVisibility", typeof(Visibility), typeof(Track), new PropertyMetadata(Visibility.Collapsed));

        public void UpdateVisualMeta()
        {
            var index = 0;
            while (index <= Items.Count - 1)
            {
                if (Items[index] is IVisualNote child)
                {
                    child.VisualIndex = index;
                    child.ParentNote = this;
                    child.MusicTheory = MusicTheory;
                }
                index++;
            }
        }
    }
}
