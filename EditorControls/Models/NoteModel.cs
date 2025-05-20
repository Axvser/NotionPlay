using NotionPlay.VisualComponents.Enums;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(NoteModel))]
    public class NoteModel
    {
        public Notes Note { get; set; } = Notes.None;
        public FrequencyLevels FrequencyLevel { get; set; } = FrequencyLevels.None;
        public DurationTypes DurationType { get; set; } = DurationTypes.None;

        public NoteModel DeepCopy()
        {
            return new NoteModel
            {
                Note = this.Note,
                FrequencyLevel = this.FrequencyLevel,
                DurationType = this.DurationType
            };
        }
    }
}
