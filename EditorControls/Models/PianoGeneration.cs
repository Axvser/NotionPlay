using NotionPlay.VisualComponents.Enums;
using System.Text.Json.Serialization;
using WindowsInput.Native;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(PianoGeneration))]
    public class PianoGeneration
    {
        public VirtualKeyCode Key { get; set; } = VirtualKeyCode.NONAME;
        public DurationTypes DurationType { get; set; } = DurationTypes.None;
        public int Span { get; set; } = 1000;
    }
}
