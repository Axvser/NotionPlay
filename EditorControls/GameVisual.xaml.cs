using Microsoft.Win32;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.EditorControls.Models;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["#99FFFFFF"])]
    [Theme(nameof(Background), typeof(Dark), ["#AA1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["Black"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    public partial class GameVisual : Popup
    {
        public async Task LoadSingleSnapshotAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "选择快照",
                Multiselect = false,
                InitialDirectory = FileHelper.SnapshotsFolder,
            };

            if (openFileDialog.ShowDialog() is true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    await using var fileStream = File.OpenRead(filePath);
                    var viewModel = await JsonSerializer
                        .DeserializeAsync<SimulationSequenceModel>(fileStream, jsonOptions)
                        .ConfigureAwait(false);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ViewModel.SelectedSimulation = viewModel ?? SimulationSequenceModel.Empty;
                        ViewModel.CurrentIndex = 0;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"⚠ {ex}");
                }
            }
        }
        public void ChangeState()
        {
            if (!CanSimulate)
            {
                IsOpen = false;
                return;
            }
            IsOpen = IsOpen != true;
        }

        public bool IsPiano
        {
            get { return (bool)GetValue(IsPianoProperty); }
            set { SetValue(IsPianoProperty, value); }
        }
        public static readonly DependencyProperty IsPianoProperty =
            DependencyProperty.Register("IsPiano", typeof(bool), typeof(GameVisual), new PropertyMetadata(false, (s, e) =>
            {
                if (s is GameVisual visual)
                {
                    visual.ViewModel.IsPiano = (bool)e.NewValue;
                    visual.ModelControlSymbol = (bool)e.NewValue ? SVG_Piano : SVG_Auto;
                }
            }));

        public GameVisualViewModel ViewModel
        {
            get { return (GameVisualViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(GameVisualViewModel), typeof(GameVisual), new PropertyMetadata(GameVisualViewModel.Default, (s, e) =>
            {
                if (s is GameVisual visual)
                {
                    visual.IsPiano = ((GameVisualViewModel)e.NewValue).IsPiano;
                }
            }));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GameVisual), new PropertyMetadata(new CornerRadius(10)));

        public string TaskControlSymbol
        {
            get { return (string)GetValue(TaskControlSymbolProperty); }
            set { SetValue(TaskControlSymbolProperty, value); }
        }
        public static readonly DependencyProperty TaskControlSymbolProperty =
            DependencyProperty.Register("TaskControlSymbol", typeof(string), typeof(GameVisual), new PropertyMetadata(SVG_Start));

        public string ModelControlSymbol
        {
            get { return (string)GetValue(ModelControlSymbolProperty); }
            set { SetValue(ModelControlSymbolProperty, value); }
        }
        public static readonly DependencyProperty ModelControlSymbolProperty =
            DependencyProperty.Register("ModelControlSymbol", typeof(string), typeof(GameVisual), new PropertyMetadata(SVG_Auto));

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(GameVisual), new PropertyMetadata(Brushes.White));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(GameVisual), new PropertyMetadata(Brushes.Black));

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };


        private void HideSelf(object sender, RoutedEventArgs e)
        {
            ChangeState();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public partial class GameVisual
    {
        public void StartOrStop(object sender, RoutedEventArgs e)
        {
            if (CanEdit)
            {
                SubmitSimulation(ViewModel);
            }
            else
            {
                StopSimulation();
            }
        }
        public void ToZero(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            ViewModel.CurrentIndex = 0;
        }
        public void VisualPiano(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            IsPiano = !IsPiano;
        }
        private async void SelectSong(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            await LoadSingleSnapshotAsync();
        }
        public void PlusScale(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSimulation.Scale = Math.Clamp(ViewModel.SelectedSimulation.Scale + 0.1, 0, double.MaxValue);
            ViewModel.Scale = (((int)(ViewModel.SelectedSimulation.Scale * 10)) / 10.0).ToString();
        }
        public void MinuScale(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSimulation.Scale = Math.Clamp(ViewModel.SelectedSimulation.Scale - 0.1, 0, double.MaxValue);
            ViewModel.Scale = (((int)(ViewModel.SelectedSimulation.Scale * 10)) / 10.0).ToString();
        }
    }

    public partial class GameVisual
    {
        private Point _dragStartPopupPosition;
        private Point _dragStartMousePosition;
        private bool _isDragging;

        private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            _dragStartPopupPosition = new Point(HorizontalOffset, VerticalOffset);
            _dragStartMousePosition = e.GetPosition(Application.Current.MainWindow); // 使用主窗口作为参考
            _isDragging = true;
            border.CaptureMouse();
            e.Handled = true;
        }
        private void TopBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                // 获取相对于主窗口的当前鼠标位置
                Point currentMousePosition = e.GetPosition(Application.Current.MainWindow);

                // 计算总偏移量（基于初始位置）
                double totalOffsetX = currentMousePosition.X - _dragStartMousePosition.X;
                double totalOffsetY = currentMousePosition.Y - _dragStartMousePosition.Y;

                // 直接设置新位置（而不是累加）
                HorizontalOffset = _dragStartPopupPosition.X + totalOffsetX;
                VerticalOffset = _dragStartPopupPosition.Y + totalOffsetY;

                e.Handled = true;
            }
        }
        private void TopBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                ((Border)sender).ReleaseMouseCapture();
                e.Handled = true;
            }
        }
    }

    public partial class GameVisual
    {
        public const string SVG_Start = "M924.059071 466.804651c-1.883153-38.201592-23.71703-72.597407-57.490703-90.554597L259.482496 50.839124c-33.877363-18.192668-74.843143-17.130342-107.729759 2.796968-33.221771 19.646342-53.365827 55.596846-52.777802 94.190454l4.364367 729.352372c-0.058869 40.135587 22.20917 76.970471 57.771671 95.578569a105.509444 105.509444 0 0 0 50.897993 12.778685 110.309981 110.309981 0 0 0 61.222223-18.838225l602.848609-404.13076c31.703207-21.277962 49.913268-57.630518 47.979273-95.762536z m-746.979759 459.647329l1.345969-832.288954 692.623186 371.223406-693.969155 461.065548z m50.880599-79.727969";
        public const string SVG_Stop = "M792 224.7v574.6H232V224.7h560m16-64H216c-26.5 0-48 21.5-48 48v606.6c0 26.5 21.5 48 48 48h592c26.5 0 48-21.5 48-48V208.7c0-26.5-21.5-48-48-48z";
        public const string SVG_Auto = "M343.134964 724.45563a44.688952 44.688952 0 0 0 11.721692-12.362723 115.38541 115.38541 0 0 0 9.157572-17.033084c2.56412-6.135573 5.12824-12.454298 7.783937-19.04775l32.051502-81.410817h206.961132l42.949014 108.242503a44.872104 44.872104 0 0 0 12.087995 19.505629 28.571625 28.571625 0 0 0 21.520295 6.685028c13.553207 0 22.161325-3.937756 25.824354-11.721693a36.630289 36.630289 0 0 0-0.824182-28.571625l-164.8363-433.611044a35.165077 35.165077 0 0 0-14.102661-18.315145 47.344648 47.344648 0 0 0-26.556959-6.685028 43.681619 43.681619 0 0 0-25.366475 6.685028 35.439804 35.439804 0 0 0-13.736359 18.315145L303.665827 687.275886c-5.677695 16.025751-5.677695 27.472717 0 32.875685a32.143078 32.143078 0 0 0 24.267567 9.157572 26.190657 26.190657 0 0 0 15.20157-4.853513zM507.696536 302.199975l89.011602 242.584088H418.593359z M754.401532 910.629073a462.640548 462.640548 0 0 1-657.42211-191.576411 468.501395 468.501395 0 0 1 2.380969-426.926017v-0.732605a19.68878 19.68878 0 0 0 1.556787-6.776604 22.619203 22.619203 0 0 0-22.619203-22.527628 21.795022 21.795022 0 0 0-21.062416 15.018419A526.1941 526.1941 0 0 0 0.000733 503.666563V511.999954a509.344167 509.344167 0 0 0 109.341412 316.210969l4.578786 5.76927a511.084105 511.084105 0 0 0 397.988089 190.019624H523.081258a509.985197 509.985197 0 0 0 253.481599-74.817365l30.128412 51.923434L836.728106 888.284597l-112.546562-29.853686z m160.165938-714.840087c-1.465212-1.92309-3.021999-3.754605-4.578786-5.677695A511.084105 511.084105 0 0 0 511.90902 0.000092h-11.263814A509.06944 509.06944 0 0 0 247.255182 75.000608l-30.128412-52.01501-29.945261 112.546562 112.546562 30.036837-30.219988-52.289737a462.548973 462.548973 0 0 1 657.422109 191.667986 468.318243 468.318243 0 0 1-2.014666 426.834441V732.605869a20.696113 20.696113 0 0 0-1.556787 6.776603 22.710779 22.710779 0 0 0 22.619204 22.619204 21.886598 21.886598 0 0 0 21.062416-15.109994 526.1941 526.1941 0 0 0 57.14325-226.558337v-0.64103-7.600785a509.06944 509.06944 0 0 0-109.616139-316.302544z";
        public const string SVG_Piano = "M906.453 606.922h-793.758c-8.227 0-15.398-4.641-18.141-12.445-2.531-7.805 0.211-16.031 6.75-20.883l580.922-428.625c3.164-2.32 7.172-4.219 11.18-4.219 0 0 0 0 0 0 10.758 0 103.148 2.109 136.898 51.68 9.914 14.766 18.773 40.289 4.008 76.992-32.273 80.789-9.492 109.477 25.102 152.508 4.219 5.273 8.648 10.969 13.078 16.664 7.594 9.703 14.766 18.141 21.727 26.156 29.953 34.594 53.367 61.383 30.164 129.727-2.531 7.805-9.703 12.445-17.93 12.445zM170.492 568.953h722.039c9.914-35.859-1.898-50.625-27-79.734-6.961-8.016-14.766-16.875-22.992-27.633-4.219-5.484-8.438-10.758-12.656-15.82-36.703-45.773-71.297-88.805-30.797-190.266 6.961-17.297 6.75-30.797-0.211-41.133-17.508-25.734-73.828-33.75-99.352-34.805l-529.031 389.391z\r\n                                     M843.805 768.711h-670.781c-45.352 0-82.477-36.914-82.477-82.477v-97.875c0-10.547 8.438-18.984 18.984-18.984h797.555c10.547 0 18.984 8.438 18.984 18.984v97.875c0 45.563-36.914 82.477-82.266 82.477zM128.727 607.555v78.891c0 24.469 19.828 44.297 44.297 44.297h670.781c24.469 0 44.297-19.828 44.297-44.297v-78.891h-759.375z\r\n                                     M225.336 882.828h-18.352c-18.773 0-34.172-15.398-34.172-34.172v-98.93c0-9.281 7.594-16.875 16.875-16.875h52.945c9.281 0 16.875 7.594 16.875 16.875v98.93c0 18.773-15.188 34.172-34.172 34.172zM206.562 766.602v82.055c0 0.211 0.211 0.422 0.422 0.422h18.352c0.211 0 0.422-0.211 0.422-0.422v-82.055h-19.195z\r\n                                     M480.148 882.828h-18.352c-18.773 0-34.172-15.398-34.172-34.172v-98.93c0-9.281 7.594-16.875 16.875-16.875h52.945c9.281 0 16.875 7.594 16.875 16.875v98.93c0 18.773-15.398 34.172-34.172 34.172zM461.375 766.602v82.055c0 0.211 0.211 0.422 0.422 0.422h18.352c0.211 0 0.422-0.211 0.422-0.422v-82.055h-19.195z\r\n                                     M842.961 882.828h-18.352c-18.773 0-34.172-15.398-34.172-34.172v-98.93c0-9.281 7.594-16.875 16.875-16.875h52.945c9.281 0 16.875 7.594 16.875 16.875v98.93c0 18.773-15.398 34.172-34.172 34.172zM824.188 766.602v82.055c0 0.211 0.211 0.422 0.422 0.422h18.352c0.211 0 0.422-0.211 0.422-0.422v-82.055h-19.195z\r\n                                     M673.789 592.156c-10.336 0-18.773-7.383-18.984-17.719l-4.43-266.203c-0.211-10.547 8.227-19.617 18.773-19.617 0.211 0 0.211 0 0.422 0 10.336 0 18.773 9.070 18.984 19.406l4.43 265.57c0.211 10.547-8.227 18.773-18.773 18.773-0.211-0.211-0.422-0.211-0.422-0.211z";
    }
}
