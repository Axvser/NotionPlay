using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
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
    [FocusModule]
    public partial class Paragraph : ItemsControl, IVisualNote, ISimulable
    {
        public event Action? Saved;
        public void RunSaved()
        {
            Saved?.Invoke();
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
        public Func<Task> GetSimulation(CancellationTokenSource source)
        {
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
            return func;
        }
        private List<SimulationAtom> CalculateAtoms()
        {
            List<SimulationAtom> atoms = [];
            foreach (var child in Items)
            {
                if (child is Track track)
                {
                    var atomCounter = 0;
                    foreach (var item in track.Items)
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
            var track = new Track() { MusicTheory = MusicTheory };
            Items.Add(track);
        }

        private void Option2_Click(object sender, RoutedEventArgs e)
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
    public partial class Paragraph
    {
        public InputSimulator Simulator { get; } = new();
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; } = 0;
        public VisualTypes VisualType { get; set; } = VisualTypes.Paragraph;

        internal Visibility BottomBorderVisibility
        {
            get { return (Visibility)GetValue(BottomBorderVisibilityProperty); }
            set { SetValue(BottomBorderVisibilityProperty, value); }
        }
        internal static readonly DependencyProperty BottomBorderVisibilityProperty =
            DependencyProperty.Register("BottomBorderVisibility", typeof(Visibility), typeof(Paragraph), new PropertyMetadata(Visibility.Collapsed));

        internal Visibility TopBorderVisibility
        {
            get { return (Visibility)GetValue(TopBorderVisibilityProperty); }
            set { SetValue(TopBorderVisibilityProperty, value); }
        }
        internal static readonly DependencyProperty TopBorderVisibilityProperty =
            DependencyProperty.Register("TopBorderVisibility", typeof(Visibility), typeof(Paragraph), new PropertyMetadata(Visibility.Collapsed));
    }
}
