using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using System.Diagnostics;
using System.Windows.Controls;
using WindowsInput;
using WindowsInput.Native;

namespace NotionPlay.VisualComponents
{
    public partial class Track : StackPanel, IVisualNote, ISimulable
    {
        public Track()
        {
            InitializeComponent();
        }

        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            async Task func()
            {
                List<(VirtualKeyCode, int)> notes = [];
                foreach (var item in Children)
                {
                    if (item is SingleNote note && KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var key))
                    {
                        notes.Add((key, MusicTheory.GetSpan(note.DurationType)));
                    }
                }
                try
                {
                    foreach (var note in notes)
                    {
                        Simulator.Keyboard.KeyDown(note.Item1);
                        await Task.Delay(note.Item2, source.Token);
                        Simulator.Keyboard.KeyUp(note.Item1);
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
                    }
                }
            }
            return (func, source);
        }
    }

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
