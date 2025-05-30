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

        public List<VirtualKeyCode> Downs { get; set; } = []; // 原子需要按下的按键
        public List<VirtualKeyCode> Ups { get; set; } = []; // 原子需要释放的按键

        public void Act()
        {
            if (Downs.Count > 0)
            {
                foreach (var key in Downs)
                {
                    Simulator.Keyboard.KeyDown(key);
                }
            }
            if (Ups.Count > 0)
            {
                foreach (var key in Ups)
                {
                    Simulator.Keyboard.KeyUp(key);
                }
            }
        }

        public void Release()
        {
            if (Downs.Count > 0)
            {
                foreach (var key in Downs)
                {
                    Simulator.Keyboard.KeyUp(key);
                }
            }
            if (Ups.Count > 0)
            {
                foreach (var key in Ups)
                {
                    Simulator.Keyboard.KeyUp(key);
                }
            }
        }
    }
}
