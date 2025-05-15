using System.Text.Json.Serialization;
using WindowsInput.Native;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(SimulationModel))]
    public class SimulationModel
    {
        public List<VirtualKeyCode> Keys { get; set; } = [];
        public int Span { get; set; } = 0;
    }
}
