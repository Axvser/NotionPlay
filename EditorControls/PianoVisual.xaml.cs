using MinimalisticWPF.TransitionSystem;
using NotionPlay.EditorControls.Models;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsInput.Native;

namespace NotionPlay.EditorControls
{
    public partial class PianoVisual : UserControl
    {
        public PianoVisual()
        {
            InitializeComponent();
            _floatingKeyPool.Initialize(21);
        }

        private readonly ObjectPool<PianoFloatingKey> _floatingKeyPool = new();

        public void ShowFloatingKey(SimulationModel simulation)
        {
            foreach (var value in simulation.PianoValues)
            {
                if (value.Key is VirtualKeyCode.NONAME) continue;

                var floatingKey = _floatingKeyPool.Get();
                var trackHeight = GetKeyHeight(value.DurationType);
                floatingKey.Height = trackHeight;
                Grid.SetColumn(floatingKey, GetKeyPosition(value.Key));
                Transition.Create<PianoFloatingKey>()
                    .SetProperty(b => b.RenderTransform, [new TranslateTransform(0, 270)])
                    .SetParams((p) =>
                    {
                        p.Start += (s, e) =>
                        {
                            floatingKey.Visibility = Visibility.Visible;
                            container.Children.Add(floatingKey);
                        };
                        p.Duration = value.Span / 1000d;
                        p.Completed += (s, e) =>
                        {
                            floatingKey.Recycle();
                            container.Children.Remove(floatingKey);
                            _floatingKeyPool.Return(floatingKey);
                        };
                    })
                    .Start(floatingKey);
            }
        }

        public static int GetKeyPosition(VirtualKeyCode key)
        {
            return key switch
            {
                VirtualKeyCode.VK_Z => 0,
                VirtualKeyCode.VK_X => 1,
                VirtualKeyCode.VK_C => 2,
                VirtualKeyCode.VK_V => 3,
                VirtualKeyCode.VK_B => 4,
                VirtualKeyCode.VK_N => 5,
                VirtualKeyCode.VK_M => 6,
                VirtualKeyCode.VK_A => 7,
                VirtualKeyCode.VK_S => 8,
                VirtualKeyCode.VK_D => 9,
                VirtualKeyCode.VK_F => 10,
                VirtualKeyCode.VK_G => 11,
                VirtualKeyCode.VK_H => 12,
                VirtualKeyCode.VK_J => 13,
                VirtualKeyCode.VK_Q => 14,
                VirtualKeyCode.VK_W => 15,
                VirtualKeyCode.VK_E => 16,
                VirtualKeyCode.VK_R => 17,
                VirtualKeyCode.VK_T => 18,
                VirtualKeyCode.VK_Y => 19,
                VirtualKeyCode.VK_U => 20,
                _ => 21,
            };
        }
        public static double GetKeyHeight(DurationTypes durationTypes) => durationTypes switch
        {
            DurationTypes.SixtyFour => 3,
            DurationTypes.ThirtyTwo => 6,
            DurationTypes.Sixteen => 12,
            DurationTypes.Eight => 24,
            DurationTypes.Four => 48,
            DurationTypes.Two => 96,
            DurationTypes.One => 192,
            _ =>0
        };

        public partial class PianoFloatingKey : Border, IRecyclable<PianoFloatingKey>
        {
            public PianoFloatingKey()
            {
                VerticalAlignment = VerticalAlignment.Top;
                HorizontalAlignment = HorizontalAlignment.Center;
                Width = 15;
                Height = 0;
                Background = Brushes.Cyan;
                CornerRadius = new(5);
            }
            public void Recycle()
            {
                RenderTransform = Transform.Identity;
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
