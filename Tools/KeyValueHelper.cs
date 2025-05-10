using NotionPlay.VisualComponents.Enums;
using WindowsInput.Native;

namespace NotionPlay.Tools
{
    public class KeyValueHelper
    {
        public static Dictionary<VirtualKeyCode, string> HighFrequencyStrings { get; private set; } = new()
        {
            [VirtualKeyCode.VK_Q] = "Q",
            [VirtualKeyCode.VK_W] = "W",
            [VirtualKeyCode.VK_E] = "E",
            [VirtualKeyCode.VK_R] = "R",
            [VirtualKeyCode.VK_T] = "T",
            [VirtualKeyCode.VK_Y] = "Y",
            [VirtualKeyCode.VK_U] = "U",
        };
        public static Dictionary<VirtualKeyCode, string> MediumFrequencyStrings { get; private set; } = new()
        {
            [VirtualKeyCode.VK_A] = "A",
            [VirtualKeyCode.VK_S] = "S",
            [VirtualKeyCode.VK_D] = "D",
            [VirtualKeyCode.VK_F] = "F",
            [VirtualKeyCode.VK_G] = "G",
            [VirtualKeyCode.VK_H] = "H",
            [VirtualKeyCode.VK_J] = "J",
        };
        public static Dictionary<VirtualKeyCode, string> LowFrequencyStrings { get; private set; } = new()
        {
            [VirtualKeyCode.VK_Z] = "Z",
            [VirtualKeyCode.VK_X] = "X",
            [VirtualKeyCode.VK_C] = "C",
            [VirtualKeyCode.VK_V] = "V",
            [VirtualKeyCode.VK_B] = "B",
            [VirtualKeyCode.VK_N] = "N",
            [VirtualKeyCode.VK_M] = "M",
        };
        public static Dictionary<char, VirtualKeyCode> HighFrequencyKeyCodes { get; private set; } = new()
        {
            ['Q'] = VirtualKeyCode.VK_Q,
            ['W'] = VirtualKeyCode.VK_W,
            ['E'] = VirtualKeyCode.VK_E,
            ['R'] = VirtualKeyCode.VK_R,
            ['T'] = VirtualKeyCode.VK_T,
            ['Y'] = VirtualKeyCode.VK_Y,
            ['U'] = VirtualKeyCode.VK_U,
        };
        public static Dictionary<char, VirtualKeyCode> MediumFrequencyKeyCodes { get; private set; } = new()
        {
            ['A'] = VirtualKeyCode.VK_A,
            ['S'] = VirtualKeyCode.VK_S,
            ['D'] = VirtualKeyCode.VK_D,
            ['F'] = VirtualKeyCode.VK_F,
            ['G'] = VirtualKeyCode.VK_G,
            ['H'] = VirtualKeyCode.VK_H,
            ['J'] = VirtualKeyCode.VK_J,
        };
        public static Dictionary<char, VirtualKeyCode> LowFrequencyKeyCodes { get; private set; } = new()
        {
            ['Z'] = VirtualKeyCode.VK_Z,
            ['X'] = VirtualKeyCode.VK_X,
            ['C'] = VirtualKeyCode.VK_C,
            ['V'] = VirtualKeyCode.VK_V,
            ['B'] = VirtualKeyCode.VK_B,
            ['N'] = VirtualKeyCode.VK_N,
            ['M'] = VirtualKeyCode.VK_M,
        };
        public static Dictionary<(Notes, FrequencyLevels), VirtualKeyCode> NoteToVirtualKeyCode { get; private set; } = new()
        {
            [(Notes.Do, FrequencyLevels.High)] = VirtualKeyCode.VK_Q,
            [(Notes.Do, FrequencyLevels.Middle)] = VirtualKeyCode.VK_A,
            [(Notes.Do, FrequencyLevels.Low)] = VirtualKeyCode.VK_Z,

            [(Notes.Re, FrequencyLevels.High)] = VirtualKeyCode.VK_W,
            [(Notes.Re, FrequencyLevels.Middle)] = VirtualKeyCode.VK_S,
            [(Notes.Re, FrequencyLevels.Low)] = VirtualKeyCode.VK_X,

            [(Notes.Mi, FrequencyLevels.High)] = VirtualKeyCode.VK_E,
            [(Notes.Mi, FrequencyLevels.Middle)] = VirtualKeyCode.VK_D,
            [(Notes.Mi, FrequencyLevels.Low)] = VirtualKeyCode.VK_C,

            [(Notes.Fa, FrequencyLevels.High)] = VirtualKeyCode.VK_R,
            [(Notes.Fa, FrequencyLevels.Middle)] = VirtualKeyCode.VK_F,
            [(Notes.Fa, FrequencyLevels.Low)] = VirtualKeyCode.VK_V,

            [(Notes.Sol, FrequencyLevels.High)] = VirtualKeyCode.VK_T,
            [(Notes.Sol, FrequencyLevels.Middle)] = VirtualKeyCode.VK_G,
            [(Notes.Sol, FrequencyLevels.Low)] = VirtualKeyCode.VK_B,

            [(Notes.La, FrequencyLevels.High)] = VirtualKeyCode.VK_Y,
            [(Notes.La, FrequencyLevels.Middle)] = VirtualKeyCode.VK_H,
            [(Notes.La, FrequencyLevels.Low)] = VirtualKeyCode.VK_N,

            [(Notes.Si, FrequencyLevels.High)] = VirtualKeyCode.VK_U,
            [(Notes.Si, FrequencyLevels.Middle)] = VirtualKeyCode.VK_J,
            [(Notes.Si, FrequencyLevels.Low)] = VirtualKeyCode.VK_M,
        };
        public static Dictionary<VirtualKeyCode, (Notes, FrequencyLevels)> VirtualKeyCodeToNote { get; private set; } = new()
        {
            [VirtualKeyCode.VK_Q] = (Notes.Do, FrequencyLevels.High),
            [VirtualKeyCode.VK_A] = (Notes.Do, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_Z] = (Notes.Do, FrequencyLevels.Low),

            [VirtualKeyCode.VK_W] = (Notes.Re, FrequencyLevels.High),
            [VirtualKeyCode.VK_S] = (Notes.Re, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_X] = (Notes.Re, FrequencyLevels.Low),

            [VirtualKeyCode.VK_E] = (Notes.Mi, FrequencyLevels.High),
            [VirtualKeyCode.VK_D] = (Notes.Mi, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_C] = (Notes.Mi, FrequencyLevels.Low),

            [VirtualKeyCode.VK_R] = (Notes.Fa, FrequencyLevels.High),
            [VirtualKeyCode.VK_F] = (Notes.Fa, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_V] = (Notes.Fa, FrequencyLevels.Low),

            [VirtualKeyCode.VK_T] = (Notes.Sol, FrequencyLevels.High),
            [VirtualKeyCode.VK_G] = (Notes.Sol, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_B] = (Notes.Sol, FrequencyLevels.Low),

            [VirtualKeyCode.VK_Y] = (Notes.La, FrequencyLevels.High),
            [VirtualKeyCode.VK_H] = (Notes.La, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_N] = (Notes.La, FrequencyLevels.Low),

            [VirtualKeyCode.VK_U] = (Notes.Si, FrequencyLevels.High),
            [VirtualKeyCode.VK_J] = (Notes.Si, FrequencyLevels.Middle),
            [VirtualKeyCode.VK_M] = (Notes.Si, FrequencyLevels.Low),
        };

        public static bool TryGetKeyCode((Notes, FrequencyLevels) tuple, out VirtualKeyCode keyCode)
        {
            if (NoteToVirtualKeyCode.TryGetValue(tuple, out var value))
            {
                keyCode = value;
                return true;
            }
            keyCode = VirtualKeyCode.NONAME;
            return false;
        }
        public static bool TryGetNote(VirtualKeyCode keyCode, out (Notes, FrequencyLevels)? tuple)
        {
            if (VirtualKeyCodeToNote.TryGetValue(keyCode, out var value))
            {
                tuple = value;
                return true;
            }
            tuple = null;
            return false;
        }
        public static bool TryGetKeyCode(char key, out VirtualKeyCode keyCode)
        {
            var upperKey = char.ToUpper(key);
            return HighFrequencyKeyCodes.TryGetValue(upperKey, out keyCode) ||
                   MediumFrequencyKeyCodes.TryGetValue(upperKey, out keyCode) ||
                   LowFrequencyKeyCodes.TryGetValue(upperKey, out keyCode);
        }
        public static bool TryGetKeyString(VirtualKeyCode keyCode, out string? key)
        {
            return HighFrequencyStrings.TryGetValue(keyCode, out key) ||
                   MediumFrequencyStrings.TryGetValue(keyCode, out key) ||
                   LowFrequencyStrings.TryGetValue(keyCode, out key);
        }
    }
}
