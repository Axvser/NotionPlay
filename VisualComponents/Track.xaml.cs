using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput;

namespace NotionPlay.VisualComponents
{
    public partial class Track : ItemsControl, IVisualNote, ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            async Task func()
            {
                var canKey = CanSimulate;
                var canPre = CanPreview;
                var canHig = CanHightLight;
                List<(int, Action, Action)> notes = [];
                foreach (var item in Items)
                {
                    if (item is SingleNote note && KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var key))
                    {
                        void action1()
                        {
                            if (canHig) note.Background = note.SimulatingBrush;
                            if (canKey) Simulator.Keyboard.KeyDown(key);
                            if (canPre) AudioHelper.PlayNote(key);
                        }
                        void action2()
                        {
                            if (canHig) note.Background = note.CurrentTheme == typeof(Dark) ? note.DarkBackground : note.LightBackground;
                            if (canKey) Simulator.Keyboard.KeyUp(key);
                            if (canPre) AudioHelper.StopNote(key);
                        }
                        notes.Add((MusicTheory.GetSpan(note.DurationType), action1, action2));
                    }
                }
                try
                {
                    foreach (var note in notes)
                    {

                        note.Item2.Invoke();
                        await Task.Delay(note.Item1, source.Token);
                        note.Item3.Invoke();
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
                        note.Item3.Invoke();
                    }
                }
            }
            return (func, source);
        }

        private void Bottom_MouseEnter(object sender, MouseEventArgs e)
        {
            BottomBorderVisibility = Visibility.Visible;
        }
        private void Bottom_MouseLeave(object sender, MouseEventArgs e)
        {
            BottomBorderVisibility = Visibility.Collapsed;
        }
        private void Top_MouseEnter(object sender, MouseEventArgs e)
        {
            TopBorderVisibility = Visibility.Visible;
        }
        private void Top_MouseLeave(object sender, MouseEventArgs e)
        {
            TopBorderVisibility = Visibility.Collapsed;
        }

        private void Option1_Click(object sender, RoutedEventArgs e)
        {
            var note = new SingleNote(Notes.None, DurationTypes.Sixteen, FrequencyLevels.Middle) { MusicTheory = MusicTheory, ParentNote = this };
            Items.Add(note);
        }
        private void Option2_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        }
    }

    [Theme(nameof(Background), typeof(Dark), ["#01ffffff"])]
    [Theme(nameof(Background), typeof(Light), ["#01ffffff"])]
    [Hover([nameof(Background)])]
    public partial class Track
    {
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
