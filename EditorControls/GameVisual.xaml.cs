using Microsoft.Win32;
using MinimalisticWPF.Controls;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.EditorControls.Models;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["#99FFFFFF"])]
    [Theme(nameof(Background), typeof(Dark), ["#AA1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["Black"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    public partial class GameVisual : Window
    {
        public static GameVisual Instance { get; set; } = new();

        public async Task LoadMultipleSnapshots()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "选择多个快照",
                Multiselect = true,
                InitialDirectory = FileHelper.SnapshotsFolder,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var viewModels = new List<SimulationSequenceModel>();

                foreach (var filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        await using var fileStream = File.OpenRead(filePath);
                        var viewModel = await JsonSerializer.DeserializeAsync<SimulationSequenceModel>(fileStream, jsonOptions);
                        if (viewModel != null && viewModel != SimulationSequenceModel.Empty)
                        {
                            viewModels.Add(viewModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        NotificationBox.Confirm($"⚠ 无法加载快照文件: {Path.GetFileName(filePath)}\n错误: {ex.Message}", "错误");
                        continue;
                    }
                }
                ViewModel.SimulationSequences.Clear();
                foreach (var viewModel in viewModels)
                {
                    ViewModel.SimulationSequences.Add(viewModel);
                }
            }
        }
        public static void ChangeState()
        {
            Instance.Visibility = Instance.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
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

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };


        private void HideSelf(object sender, RoutedEventArgs e)
        {
            ChangeState();
        }
        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }

    public partial class GameVisual
    {
        public void StartOrStop(object sender, RoutedEventArgs e)
        {
            SubmitSimulation(ViewModel);
        }
        public void PlusScale(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSimulation.Scale = Math.Clamp(ViewModel.SelectedSimulation.Scale + 0.1, 0, double.MaxValue);
            ViewModel.Scale = ViewModel.SelectedSimulation.Scale.ToString();
        }
        public void MinuScale(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSimulation.Scale = Math.Clamp(ViewModel.SelectedSimulation.Scale - 0.1, 0, double.MaxValue);
            ViewModel.Scale = ViewModel.SelectedSimulation.Scale.ToString();
        }
    }

    public partial class GameVisual
    {
        public const string SVG_Start = "M924.059071 466.804651c-1.883153-38.201592-23.71703-72.597407-57.490703-90.554597L259.482496 50.839124c-33.877363-18.192668-74.843143-17.130342-107.729759 2.796968-33.221771 19.646342-53.365827 55.596846-52.777802 94.190454l4.364367 729.352372c-0.058869 40.135587 22.20917 76.970471 57.771671 95.578569a105.509444 105.509444 0 0 0 50.897993 12.778685 110.309981 110.309981 0 0 0 61.222223-18.838225l602.848609-404.13076c31.703207-21.277962 49.913268-57.630518 47.979273-95.762536z m-746.979759 459.647329l1.345969-832.288954 692.623186 371.223406-693.969155 461.065548z m50.880599-79.727969";
        public const string SVG_Stop = "M792 224.7v574.6H232V224.7h560m16-64H216c-26.5 0-48 21.5-48 48v606.6c0 26.5 21.5 48 48 48h592c26.5 0 48-21.5 48-48V208.7c0-26.5-21.5-48-48-48z";
    }
}
