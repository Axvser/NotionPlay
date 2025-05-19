using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using System.Text.Json.Serialization;
using WindowsInput.Native;

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
            var maxAtomCount = viewModel.Notes.Select(list => CalculateAtomCount(list)).Max();
            if (maxAtomCount == 0) return;
            var startIndex = Simulations.Count; // 新元素的起始位置
            while (maxAtomCount + startIndex >= Simulations.Count)
            {
                Simulations.Add(new SimulationModel());
            }

            foreach (var trackvalues in viewModel.Notes)
            {
                var atomCount = 0;
                foreach (var note in trackvalues)
                {
                    int steps = Math.Clamp(64 / (int)note.DurationType, 1, 64);
                    for (int i = 0; i < steps; i++)
                    {
                        Simulations[atomCount + startIndex].Span = Theory.GetSpan(DurationTypes.SixtyFour);

                        if (i == 0 && KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var virtualKey))
                        {
                            Simulations[atomCount + startIndex].Keys.Add(virtualKey);
                        }

                        atomCount++;
                    }
                }
            }
        }
        private static int CalculateAtomCount(List<NoteModel> notes)
        {
            var result = 0;

            foreach (var note in notes)
            {
                result += Math.Clamp(64 / (int)note.DurationType, 1, 64);
            }

            return result;
        }
    }
}
