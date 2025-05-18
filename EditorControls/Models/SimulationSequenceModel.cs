using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using System.Text.Json.Serialization;
using WindowsInput.Native;
using NotionPlay.Interfaces;
using System.Windows;
using NotionPlay.VisualComponents.Enums;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(SimulationSequenceModel))]
    public class SimulationSequenceModel
    {
        [JsonIgnore]
        public static SimulationSequenceModel Empty { get; private set; } = new();

        public string Name { get; set; } = string.Empty;
        public int Speed { get; set; } = 80;
        public int LeftNum { get; set; } = 4;
        public int RightNum { get; set; } = 4;
        public double Scale { get; set; } = 1;
        public List<SimulationModel> Simulations { get; set; } = [];

        public static SimulationSequenceModel FromTreeItemViewModel(TreeItemViewModel viewModel)
        {
            var result = new SimulationSequenceModel()
            {
                Speed = Theory.Speed,
                LeftNum = Theory.LeftNum,
                RightNum = Theory.RightNum,
            };

            switch (viewModel.Type)
            {
                case TreeItemTypes.Project:
                    result.ParseSnapshotAtProject(viewModel);
                    break;
                case TreeItemTypes.Package:
                    result.ParseSnapshotAtPackage(viewModel);
                    break;
                case TreeItemTypes.Paragraph:
                    result.ParseSnapshotAtParagraph(viewModel);
                    break;
            }

            return result;
        }
        private void ParseSnapshotAtProject(TreeItemViewModel viewModel)
        {
            foreach (var child in viewModel.Children)
            {
                ParseSnapshotAtPackage(child);
            }
        }
        private void ParseSnapshotAtPackage(TreeItemViewModel viewModel)
        {
            foreach (var child in viewModel.Children)
            {
                ParseSnapshotAtParagraph(child);
            }
        }
        private void ParseSnapshotAtParagraph(TreeItemViewModel viewModel)
        {
            foreach (var trackvalues in viewModel.Notes)
            {
                var atomCounter = 0;
                foreach (var note in trackvalues)
                {
                    int steps = Math.Clamp(64 / (int)note.DurationType, 1, 64);
                    for (int i = 0; i < steps; i++)
                    {
                        while (atomCounter >= Simulations.Count)
                        {
                            Simulations.Add(new SimulationModel());
                        }

                        Simulations[atomCounter].Span = Theory.GetSpan(DurationTypes.SixtyFour);
                        _ = KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var virtualKey);

                        if (i == 0)
                        {
                            Simulations[atomCounter].Keys.Add(virtualKey);
                        }
                        else
                        {
                            Simulations[atomCounter].Keys.Add(VirtualKeyCode.NONAME);
                        }

                        atomCounter++;
                    }
                }
            }
        }
    }
}
