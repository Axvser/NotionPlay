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

        public static void ChangeState()
        {
            Instance.Visibility = Instance.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public GameVisualViewModel ViewModel { get; set; } = GameVisualViewModel.Default;

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GameVisual), new PropertyMetadata(new CornerRadius(10)));

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

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
    }
}
