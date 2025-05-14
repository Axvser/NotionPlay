using NotionPlay.VisualComponents.Enums;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(NoteModel))]
    public class NoteModel
    {
        private Notes Note { get; set; } = Notes.None;
        private FrequencyLevels FrequencyLevel { get; set; } = FrequencyLevels.None;
        private DurationTypes DurationType { get; set; } = DurationTypes.None;
    }
}
