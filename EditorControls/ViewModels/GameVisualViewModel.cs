using MinimalisticWPF.SourceGeneratorMark;
using NotionPlay.EditorControls.Models;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace NotionPlay.EditorControls.ViewModels
{
    [JsonSerializable(typeof(GameVisualViewModel))]
    public partial class GameVisualViewModel
    {
        [JsonIgnore]
        public static GameVisualViewModel Default { get; private set; } = new();

        [Observable]
        private SimulationSequenceModel selectedSimulation = SimulationSequenceModel.Empty;
        [Observable]
        private string name = "Default";
        [Observable]
        private string speed = "80";
        [Observable]
        private string signature = "4 / 4";
        [Observable]
        private string scale = "1";
        [Observable]
        private double progress = 0d;
        [Observable]
        private int currentIndex = 0;
        [Observable]
        private bool isPiano = false;
        [Observable]
        private Visibility infoVisibility = Visibility.Visible;
        [Observable]
        private Visibility pianoVisibility = Visibility.Hidden;
    }

    public partial class GameVisualViewModel
    {
        partial void OnIsPianoChanged(bool oldValue, bool newValue)
        {
            StopSimulation();
            CurrentIndex = 0;
            PianoVisibility = IsPiano ? Visibility.Visible : Visibility.Hidden;
            InfoVisibility = IsPiano ? Visibility.Hidden : Visibility.Visible;
        }
        partial void OnCurrentIndexChanged(int oldValue, int newValue)
        {
            if (newValue == 0 || SelectedSimulation.Simulations.Count == 0) Progress = 0d;
            else Progress = (double)(newValue + 1) / Math.Clamp(SelectedSimulation.Simulations.Count, 1, int.MaxValue);
        }
        partial void OnSelectedSimulationChanged(SimulationSequenceModel oldValue, SimulationSequenceModel newValue)
        {
            StopSimulation();
            CurrentIndex = 0;
            Name = newValue.Name;
            Speed = newValue.Speed.ToString();
            Signature = $"{newValue.LeftNum} / {newValue.RightNum}";
            Scale = newValue.Scale.ToString();
        }
    }

    public partial class GameVisualViewModel : ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            if (IsPiano)
            {
                return (async () =>
                {
                    MainWindow.UpdateGameVisualText(GameVisual.SVG_Stop);
                    if (CurrentIndex == SelectedSimulation.Simulations.Count - 1) CurrentIndex = 0;

                    MainWindow.UpdateGameVisualText(GameVisual.SVG_Start);
                }, source);
            }
            else
            {
                return (async () =>
                {
                    MainWindow.UpdateGameVisualText(GameVisual.SVG_Stop);
                    if (CurrentIndex == SelectedSimulation.Simulations.Count - 1) CurrentIndex = 0;
                    var span = (int)(SelectedSimulation.Span * SelectedSimulation.Scale);
                    for (int i = CurrentIndex; i < SelectedSimulation.Simulations.Count; i++)
                    {
                        try
                        {
                            if (source.Token.IsCancellationRequested) break;
                            CurrentIndex = i;
                            SelectedSimulation.Simulations[i].Act();
                            await Task.Delay(span, source.Token);
                        }
                        catch
                        {

                        }
                    }

                    // 从任何时刻终止时，向前遍历64个原子以确保所有按键均释放
                    var currentIndex = CurrentIndex;
                    var counter = 0;
                    while (currentIndex > -1 && currentIndex < SelectedSimulation.Simulations.Count && counter < 64)
                    {
                        SelectedSimulation.Simulations[currentIndex].Release();
                        currentIndex--;
                        counter++;
                    }
                    MainWindow.UpdateGameVisualText(GameVisual.SVG_Start);
                }, source);
            }
        }
    }

    public partial class GameVisualViewModel
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };
        private static readonly string ConfigPath = Path.Combine(FileHelper.ConfigsFolder, "GameVisualConfig.json");

        public static async Task SaveFile(GameVisualViewModel data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                var json = JsonSerializer.Serialize(data, jsonOptions);
                await File.WriteAllTextAsync(ConfigPath, json);
            }
            catch
            {

            }
        }
        public static async Task<GameVisualViewModel> FromFile()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    return Default;
                }
                var json = await File.ReadAllTextAsync(ConfigPath);
                return JsonSerializer.Deserialize<GameVisualViewModel>(json, jsonOptions)!;
            }
            catch
            {
                return Default;
            }
        }
    }
}
