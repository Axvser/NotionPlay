using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.Diagnostics;
using System.Windows.Controls;
using WindowsInput;
using WindowsInput.Native;

namespace NotionPlay.VisualComponents
{
    public partial class Track : StackPanel, IVisualNote, ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            async Task func()
            {
                List<(VirtualKeyCode, int, Action, Action)> notes = [];
                foreach (var item in Children)
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
    }

    [Theme(nameof(Background), typeof(Dark), ["Transparent"])]
    [Theme(nameof(Background), typeof(Light), ["Transparent"])]
    [Hover([nameof(Background)])]
    public partial class Track
    {
        public InputSimulator Simulator { get; } = new();
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; } = 0;
        public VisualTypes VisualType { get; set; } = VisualTypes.Track;

        public void UpdateVisualMeta()
        {
            var index = 0;
            while (index <= Children.Count - 1)
            {
                if (Children[index] is IVisualNote child)
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
