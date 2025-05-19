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

            int globalAtomCounter = 0;
            switch (viewModel.Type)
            {
                case TreeItemTypes.Project:
                    result.ParseSnapshotAtProject(viewModel, ref globalAtomCounter);
                    break;
                case TreeItemTypes.Package:
                    result.ParseSnapshotAtPackage(viewModel, ref globalAtomCounter);
                    break;
                case TreeItemTypes.Paragraph:
                    result.ParseSnapshotAtParagraph(viewModel, ref globalAtomCounter);
                    break;
            }
            return result;
        }
        private void ParseSnapshotAtProject(TreeItemViewModel viewModel, ref int globalAtomCounter)
        {
            foreach (var child in viewModel.Children)
            {
                ParseSnapshotAtPackage(child, ref globalAtomCounter);
            }
        }
        private void ParseSnapshotAtPackage(TreeItemViewModel viewModel, ref int globalAtomCounter)
        {
            foreach (var child in viewModel.Children)
            {
                ParseSnapshotAtParagraph(child, ref globalAtomCounter);
            }
        }
        private void ParseSnapshotAtParagraph(TreeItemViewModel viewModel, ref int globalAtomCounter)
        {
            foreach (var trackvalues in viewModel.Notes)
            {
                foreach (var note in trackvalues)
                {
                    int steps = Math.Clamp(64 / (int)note.DurationType, 1, 64);
                    for (int i = 0; i < steps; i++)
                    {
                        while (globalAtomCounter >= Simulations.Count)
                        {
                            Simulations.Add(new SimulationModel());
                        }

                        Simulations[globalAtomCounter].Span = Theory.GetSpan(DurationTypes.SixtyFour);

                        if (i == 0 && KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var virtualKey))
                        {
                            Simulations[globalAtomCounter].Keys.Add(virtualKey);
                        }
                        else
                        {
                            Simulations[globalAtomCounter].Keys.Add(VirtualKeyCode.NONAME);
                        }

                        globalAtomCounter++;
                    }
                }
            }
        }
    }
}
