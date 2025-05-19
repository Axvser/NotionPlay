using Microsoft.Win32;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.EditorControls.Models;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using System.IO;
using System.Runtime.InteropServices;
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

        public GameVisualViewModel ViewModel
        {
            get { return (GameVisualViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(GameVisualViewModel), typeof(GameVisual), new PropertyMetadata(GameVisualViewModel.Default));

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
        private async void SelectSong(object sender, RoutedEventArgs e)
        {
            await LoadSingleSnapshotAsync();
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
    }
}
