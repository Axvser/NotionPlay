using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Models;
using NotionPlay.VisualComponents;
using System.Text.Json.Serialization;

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
            foreach(var child in viewModel.Children)
            {
                ParseSnapshotAtPackage(child);
            }
        }
        private void ParseSnapshotAtPackage(TreeItemViewModel viewModel)
        {
            foreach(var child in viewModel.Children)
            {
                ParseSnapshotAtParagraph(child);
            }
        }
        private void ParseSnapshotAtParagraph(TreeItemViewModel viewModel)
        {

        }

    }
}
