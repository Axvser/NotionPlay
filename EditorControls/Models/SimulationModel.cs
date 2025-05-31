using System.Text.Json.Serialization;
using WindowsInput;
using WindowsInput.Native;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(SimulationModel))]
    public class SimulationModel
    {
        [JsonIgnore]
        public static readonly InputSimulator Simulator_Down = new();
        [JsonIgnore]
        public static readonly InputSimulator Simulator_Up = new();

        public List<VirtualKeyCode> Downs { get; set; } = []; // 原子需要按下的按键
        public List<VirtualKeyCode> Ups { get; set; } = []; // 原子需要释放的按键
        public List<PianoGeneration> PianoValues { get; set; } = []; // 虚拟钢琴生成所需的数据

        public void KeyDown()
        {
            foreach (var key in Downs)
            {
                Simulator_Down.Keyboard.KeyDown(key);
            }
        }
        public void KeyUp()
        {
            foreach (var key in Ups)
            {
                Simulator_Up.Keyboard.KeyUp(key);
            }
        }
    }
}
