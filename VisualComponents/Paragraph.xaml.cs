using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.Diagnostics;
using System.Windows.Controls;
using WindowsInput;

namespace NotionPlay.VisualComponents
{
    public partial class Paragraph : StackPanel, IVisualNote, ISimulable
    {
        public Paragraph()
        {
            InitializeComponent();
        }

        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            async Task func()
            {
                var span = MusicTheory.GetSpan(DurationTypes.Sixteen);
                var atoms = CalculateAtoms();
                try
                {
                    foreach (var atom in atoms)
                    {
                        atom.Run();
                        await Task.Delay(span, source.Token);
                        atom.Release();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    foreach (var atom in atoms)
                    {
                        atom.Release();
                    }
                }
            }
            return (func, source);
        }
        private List<SimulationAtom> CalculateAtoms()
        {
            List<SimulationAtom> atoms = [];
            foreach (var child in Children)
            {
                if (child is Track track)
                {
                    var atomCounter = 0;
                    foreach (var item in track.Children)
                    {
                        if (item is SingleNote note)
                        {
                            int steps = 16 / (int)note.DurationType;
                            for (int i = 0; i < steps; i++)
                            {
                                while (atomCounter >= atoms.Count)
                                {
                                    atoms.Add(new SimulationAtom());
                                }

                                atoms[atomCounter].Span = MusicTheory.GetSpan(note.DurationType);
                                _ = KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var virtualKey);

                                var currentNote = note;
                                atoms[atomCounter].KeyDowns += () =>
                                {
                                    Simulator.Keyboard.KeyDown(virtualKey);
                                    currentNote.Background = currentNote.SimulatingBrush;
                                };
                                atoms[atomCounter].KeyUps += () =>
                                {
                                    Simulator.Keyboard.KeyUp(virtualKey);
                                    currentNote.Background = currentNote.CurrentTheme == typeof(Dark)
                                        ? currentNote.DarkBackground
                                        : currentNote.LightBackground;
                                };

                                atomCounter++;
                            }
                        }
                    }
                }
            }
            return atoms;
        }
    }

    public partial class Paragraph
    {
        public InputSimulator Simulator { get; } = new();
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; } = 0;
        public VisualTypes VisualType { get; set; } = VisualTypes.Paragraph;
    }
}
