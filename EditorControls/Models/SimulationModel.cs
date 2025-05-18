using System.Text.Json.Serialization;
using WindowsInput;
using WindowsInput.Native;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(SimulationModel))]
    public class SimulationModel
    {
        [JsonIgnore]
        public static readonly InputSimulator Simulator = new();

        public List<VirtualKeyCode> Keys { get; set; } = [];
        public int Span { get; set; } = 0;

        public void KeyDown()
        {
            foreach (var key in Keys)
            {
                Simulator.Keyboard.KeyDown(key);
            }
        }
        public void KeyUp()
        {
            foreach (var key in Keys)
            {
                Simulator.Keyboard.KeyUp(key);
            }
        }
    }
}
