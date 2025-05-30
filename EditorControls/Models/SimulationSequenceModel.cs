using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents.Enums;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(SimulationSequenceModel))]
    public class SimulationSequenceModel
    {
        [JsonIgnore]
        public static SimulationSequenceModel Empty { get; private set; } = new();

        public int Span { get; set; } = 1000; // 原子耗时
        public string Name { get; set; } = string.Empty; // 快照名
        public int Speed { get; set; } = 80;
        public int LeftNum { get; set; } = 4;
        public int RightNum { get; set; } = 4;
        public double Scale { get; set; } = 1;
        public List<SimulationModel> Simulations { get; set; } = [];

        public static SimulationSequenceModel FromTreeItemViewModel(TreeItemViewModel viewModel)
        {
            var result = new SimulationSequenceModel()
            {
                Span = Theory.GetSpan(DurationTypes.SixtyFour), // 以64分音符持续时间为一个原子耗时
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
            Simulations.Add(new SimulationModel()); // 始终确保原子可以持续足够的时长，因为它比 startIndex 多偏移一个单位
            foreach (var trackvalues in viewModel.Notes)
            {
                var atomCount = 0;
                foreach (var note in trackvalues)
                {
                    int steps = Math.Clamp(64 / (int)note.DurationType, 1, 64);
                    for (int i = 0; i < steps; i++)
                    {
                        var isKeyExsist = KeyValueHelper.TryGetKeyCode((note.Note, note.FrequencyLevel), out var virtualKey);
                        if (i == 0 && isKeyExsist)
                        {
                            Simulations[atomCount + startIndex].Downs.Add(virtualKey);
                        }
                        if (i == steps - 1 && isKeyExsist)
                        {
                            if (i > 0)
                            {
                                Simulations[atomCount + startIndex].Ups.Add(virtualKey);
                            }
                            else
                            {
                                Simulations[atomCount + startIndex + 1].Ups.Add(virtualKey);
                            }
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
