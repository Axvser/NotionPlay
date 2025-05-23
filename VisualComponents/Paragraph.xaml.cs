﻿using MinimalisticWPF.SourceGeneratorMark;
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
                var span = MusicTheory.GetSpan(DurationTypes.SixtyFour);
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
                var span = MusicTheory.GetSpan(DurationTypes.SixtyFour);
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
            var canKey = CanSimulate;
            var canPre = CanPreview;
            var canHig = CanHightLight;
            foreach (var child in Items)
            {
                if (child is Track track)
                {
                    var atomCounter = 0;
                    foreach (var item in track.Items)
                    {
                        if (item is SingleNote note)
                        {
                            int steps = Math.Clamp(64 / (int)note.DurationType, 1, 64);
                            for (int i = 0; i < steps; i++)
                            {
                                while (atomCounter >= atoms.Count)
                                {
                                    atoms.Add(new SimulationAtom());
                                }

                                atoms[atomCounter].Span = MusicTheory.GetSpan(DurationTypes.SixtyFour);
                                _ = KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var virtualKey);

                                if (i == 0)
                                {
                                    atoms[atomCounter].KeyDowns += () =>
                                    {
                                        if (canKey) Simulator.Keyboard.KeyDown(virtualKey);
                                        if (canPre) AudioHelper.PlayNote(virtualKey);
                                        if (canHig) note.Background = note.SimulatingBrush;
                                    };
                                }

                                if (i == steps - 1)
                                {
                                    atoms[atomCounter].KeyUps += () =>
                                    {
                                        if (canKey) Simulator.Keyboard.KeyUp(virtualKey);
                                        if (canPre) AudioHelper.StopNote(virtualKey);
                                        if (canHig) note.Background = note.CurrentTheme == typeof(Dark)
                                            ? note.DarkBackground
                                            : note.LightBackground;
                                    };
                                }

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

    [Theme(nameof(Background), typeof(Dark), ["#01ffffff"])]
    [Theme(nameof(Background), typeof(Light), ["#01ffffff"])]
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
