using MinimalisticWPF.SourceGeneratorMark;
using NotionPlay.EditorControls.Models;
using NotionPlay.Interfaces;
using NotionPlay.Tools;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.ViewModels
{
    [JsonSerializable(typeof(GameVisualViewModel))]
    public partial class GameVisualViewModel
    {
        [JsonIgnore]
        public static GameVisualViewModel Default { get; private set; } = new();

        [Observable]
        private ObservableCollection<SimulationSequenceModel> simulationSequences = [];
        [Observable]
        private SimulationSequenceModel selectedSimulation = SimulationSequenceModel.Empty;
        [Observable]
        private string name = string.Empty;
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
    }

    public partial class GameVisualViewModel
    {
        partial void OnCurrentIndexChanged(int oldValue, int newValue)
        {
            Progress = (double)(newValue + 1) / Math.Clamp(SelectedSimulation.Simulations.Count, 1, int.MaxValue);
        }
        partial void OnSelectedSimulationChanged(SimulationSequenceModel oldValue, SimulationSequenceModel newValue)
        {
            StopSimulation();
            CurrentIndex = 0;
            name = newValue.Name;
            speed = newValue.Speed.ToString();
            signature = $"{newValue.LeftNum} / {newValue.RightNum}";
            scale = newValue.Scale.ToString();
        }
    }

    public partial class GameVisualViewModel : ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation()
        {
            var source = new CancellationTokenSource();
            return (async () =>
            {
                GameVisual.Instance.TaskControlSymbol = GameVisual.SVG_Stop;
                for (int i = CurrentIndex; i < SelectedSimulation.Simulations.Count; i++)
                {
                    try
                    {
                        if (source.Token.IsCancellationRequested) break;
                        CurrentIndex = i;
                        SelectedSimulation.Simulations[i].KeyDown();
                        await Task.Delay(SelectedSimulation.Simulations[i].Span, source.Token);
                        SelectedSimulation.Simulations[i].KeyUp();
                    }
                    catch
                    {

                    }
                }
                foreach (var simulation in SelectedSimulation.Simulations)
                {
                    simulation.KeyUp();
                }
                GameVisual.Instance.TaskControlSymbol = GameVisual.SVG_Start;
            }, source);
        }
    }

    public partial class GameVisualViewModel
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
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
