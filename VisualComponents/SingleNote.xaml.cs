using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput;

namespace NotionPlay.VisualComponents
{
    public partial class SingleNote : UserControl, IVisualNote, ISimulable
    {
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateSize();
        }

        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            KeyValueHelper.TryGetKeyCode((Note, FrequencyLevel), out var key);
            async Task func()
            {
                try
                {
                    Simulator.Keyboard.KeyDown(key);
                    await Task.Delay(MusicTheory.GetSpan(DurationType), source.Token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    Simulator.Keyboard.KeyUp(key);
                }
            }
            return (func, source);
        }
    } // 默认

    public partial class SingleNote
    {
        [Constructor]
        private void InitializeDefaultNote()
        {
            Loaded += (s, e) => UpdateNote();
            LocalHotKey.Register(this, [Key.Delete], (s, e) =>
            {
                if (NotificationBox.Choose("⚠ 删除操作不可撤销 , 确定继续吗 ?"))
                {
                    if (ParentNote is Track track)
                    {
                        track.Children.Remove(this);
                        track.UpdateVisualMeta();
                    }
                }
            });
            LocalHotKey.Register(this, [Key.W], (s, e) =>
            {
                Up_Frequency();
            });
            LocalHotKey.Register(this, [Key.S], (s, e) =>
            {
                Down_Frequency();
            });
            LocalHotKey.Register(this, [Key.D], (s, e) =>
            {
                Up_Duration();
            });
            LocalHotKey.Register(this, [Key.A], (s, e) =>
            {
                Down_Duration();
            });
            LocalHotKey.Register(this, [Key.Up], (s, e) =>
            {
                Up_Frequency();
            });
            LocalHotKey.Register(this, [Key.Down], (s, e) =>
            {
                Down_Frequency();
            });
            LocalHotKey.Register(this, [Key.Right], (s, e) =>
            {
                Up_Duration();
            });
            LocalHotKey.Register(this, [Key.Left], (s, e) =>
            {
                Down_Duration();
            });
            LocalHotKey.Register(this, [Key.D0], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.None;
            });
            LocalHotKey.Register(this, [Key.D1], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.Do;
            });
            LocalHotKey.Register(this, [Key.D2], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.Re;
            });
            LocalHotKey.Register(this, [Key.D3], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.Mi;
            });
            LocalHotKey.Register(this, [Key.D4], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.Fa;
            });
            LocalHotKey.Register(this, [Key.D5], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.Sol;
            });
            LocalHotKey.Register(this, [Key.D6], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.La;
            });
            LocalHotKey.Register(this, [Key.D7], (s, e) =>
            {
                if (!CanEdit) return;
                Note = Notes.Si;
            });
        }

        [Constructor]
        private void InitializeNote(Notes value, DurationTypes duration, FrequencyLevels level)
        {
            Note = value;
            DurationType = duration;
            FrequencyLevel = level;
            InitializeDefaultNote();
        }
    } // 构造器

    public partial class SingleNote
    {
        public InputSimulator Simulator { get; } = new();
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; } = 0;
        public VisualTypes VisualType { get; set; } = VisualTypes.SingleNote;

        public DurationTypes DurationType
        {
            get { return (DurationTypes)GetValue(DurationTypeProperty); }
            set { SetValue(DurationTypeProperty, value); }
        }
        public static readonly DependencyProperty DurationTypeProperty =
            DependencyProperty.Register("DurationType", typeof(DurationTypes), typeof(SingleNote), new PropertyMetadata(DurationTypes.Sixteen, (dp, e) =>
            {
                if (dp is SingleNote note)
                {
                    note.UpdateWidth();
                    note.UpdateDurationSymbol();
                }
            }));

        public FrequencyLevels FrequencyLevel
        {
            get { return (FrequencyLevels)GetValue(FrequencyLevelProperty); }
            set { SetValue(FrequencyLevelProperty, value); }
        }
        public static readonly DependencyProperty FrequencyLevelProperty =
            DependencyProperty.Register("FrequencyLevel", typeof(FrequencyLevels), typeof(SingleNote), new PropertyMetadata(FrequencyLevels.Middle, (dp, e) =>
            {
                if (dp is SingleNote note)
                {
                    note.UpdateLevelSymbol();
                }
            }));

        public Notes Note
        {
            get { return (Notes)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }
        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register("Note", typeof(Notes), typeof(SingleNote), new PropertyMetadata(Notes.None, (dp, e) =>
            {
                if (dp is SingleNote note)
                {
                    note.UpdateValueSymbol();
                    switch ((Notes)e.OldValue == Notes.None, (Notes)e.NewValue == Notes.None)
                    {
                        case (false, true):
                            note.FrequencyLevel = FrequencyLevels.None;
                            break;
                        case (true, false):
                            note.FrequencyLevel = FrequencyLevels.Middle;
                            break;
                    }
                }
            }));

        internal string DurationSymbol
        {
            get { return (string)GetValue(DurationSymbolProperty); }
            set { SetValue(DurationSymbolProperty, value); }
        }
        internal static readonly DependencyProperty DurationSymbolProperty =
            DependencyProperty.Register("DurationSymbol", typeof(string), typeof(SingleNote), new PropertyMetadata(string.Empty));

        internal string DurationSuffix
        {
            get { return (string)GetValue(DurationSuffixProperty); }
            set { SetValue(DurationSuffixProperty, value); }
        }
        internal static readonly DependencyProperty DurationSuffixProperty =
            DependencyProperty.Register("DurationSuffix", typeof(string), typeof(SingleNote), new PropertyMetadata(string.Empty));

        internal string TopLevelSymbol
        {
            get { return (string)GetValue(TopLevelSymbolProperty); }
            set { SetValue(TopLevelSymbolProperty, value); }
        }
        internal static readonly DependencyProperty TopLevelSymbolProperty =
            DependencyProperty.Register("TopLevelSymbol", typeof(string), typeof(SingleNote), new PropertyMetadata(string.Empty));

        internal string BottomLevelSymbol
        {
            get { return (string)GetValue(BottomLevelSymbolProperty); }
            set { SetValue(BottomLevelSymbolProperty, value); }
        }
        internal static readonly DependencyProperty BottomLevelSymbolProperty =
            DependencyProperty.Register("BottomLevelSymbol", typeof(string), typeof(SingleNote), new PropertyMetadata(string.Empty));

        internal string ValueSymbol
        {
            get { return (string)GetValue(ValueSymbolProperty); }
            set { SetValue(ValueSymbolProperty, value); }
        }
        internal static readonly DependencyProperty ValueSymbolProperty =
            DependencyProperty.Register("ValueSymbol", typeof(string), typeof(SingleNote), new PropertyMetadata(string.Empty));

        public void UpdateNote()
        {
            UpdateSize();
            UpdateWidth();
            UpdateDurationSymbol();
            UpdateLevelSymbol();
            UpdateValueSymbol();
        }

        public void Up_Note()
        {
            if (CanEdit) Note = (Notes)Math.Clamp((int)Note + 1, 0, 7);
        }
        public void Up_Frequency()
        {
            if (CanEdit && Note != Notes.None) FrequencyLevel = (FrequencyLevels)Math.Clamp((int)FrequencyLevel + 1, 1, 3);
        }
        public void Up_Duration()
        {
            if (CanEdit) DurationType = (DurationTypes)Math.Clamp((int)DurationType / 2, 1, 16);
        }
        public void Down_Note()
        {
            if (CanEdit) Note = (Notes)Math.Clamp((int)Note - 1, 0, 7);
        }
        public void Down_Frequency()
        {
            if (CanEdit && Note != Notes.None) FrequencyLevel = (FrequencyLevels)Math.Clamp((int)FrequencyLevel - 1, 1, 3);
        }
        public void Down_Duration()
        {
            if (CanEdit) DurationType = (DurationTypes)Math.Clamp((int)DurationType * 2, 1, 16);
        }

        private void UpdateSize() => FontSize = Math.Clamp(ActualHeight * FontSizeScale, 0.1d, double.MaxValue);
        private void UpdateWidth() => Width = ((double)DurationTypes.Sixteen / (double)DurationType) * UnitWidth;
        private void UpdateDurationSymbol()
        {
            switch (DurationType)
            {
                case DurationTypes.Sixteen:
                    DurationSymbol = SixteenthSymbol;
                    DurationSuffix = string.Empty;
                    break;
                case DurationTypes.Eight:
                    DurationSymbol = EighthSymbol;
                    DurationSuffix = string.Empty;
                    break;
                case DurationTypes.Two:
                    DurationSymbol = string.Empty;
                    DurationSuffix = TwoSymbol;
                    break;
                case DurationTypes.One:
                    DurationSymbol = string.Empty;
                    DurationSuffix = OneSymbol;
                    break;
                default:
                    DurationSymbol = string.Empty;
                    DurationSuffix = string.Empty;
                    break;
            }
        }
        private void UpdateLevelSymbol()
        {
            switch (FrequencyLevel)
            {
                case FrequencyLevels.High:
                    TopLevelSymbol = FrequencySymbol;
                    BottomLevelSymbol = string.Empty;
                    break;
                case FrequencyLevels.Low:
                    TopLevelSymbol = string.Empty;
                    BottomLevelSymbol = FrequencySymbol;
                    break;
                default:
                    TopLevelSymbol = string.Empty;
                    BottomLevelSymbol = string.Empty;
                    break;
            }
        }
        private void UpdateValueSymbol()
        {
            ValueSymbol = Note switch
            {
                Notes.None => "0",
                Notes.Do => "1",
                Notes.Re => "2",
                Notes.Mi => "3",
                Notes.Fa => "4",
                Notes.Sol => "5",
                Notes.La => "6",
                Notes.Si => "7",
                _ => string.Empty,
            };
        }
    } // 依赖属性

    [ClickModule]
    [FocusModule]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(BorderBrush), typeof(Dark), ["White"])]
    [Theme(nameof(BorderBrush), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Hover([nameof(BorderThickness), nameof(Foreground)])]
    public partial class SingleNote
    {
        const double UnitWidth = 15d;
        const double FontSizeScale = 0.25d;
        const string SixteenthSymbol = "=";
        const string EighthSymbol = "—";
        const string TwoSymbol = "——";
        const string OneSymbol = "——                   ——";
        const string FrequencySymbol = "▪";
    } // MinimalisticWPF - ViewEx
}
