using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Models;
using NotionPlay.VisualComponents;
using System.Text.Json.Serialization;
using System.Windows.Controls.Primitives;
using WindowsInput.Native;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(SimulationSequenceModel))]
    public class SimulationSequenceModel
    {
        public List<SimulationModel> Simulations { get; set; } = [];

        public static SimulationSequenceModel FromTreeItemViewModel(TreeItemViewModel viewModel)
        {
            var result = new SimulationSequenceModel();

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

                        Simulations[atomCounter].Span = Theory.GetSpan(note.DurationType);
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
