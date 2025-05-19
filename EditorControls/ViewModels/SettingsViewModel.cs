using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using NotionPlay.Tools;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.ViewModels
{
    [JsonSerializable(typeof(SettingsViewModel))]
    public partial class SettingsViewModel
    {
        [JsonIgnore]
        public static SettingsViewModel Default { get; private set; } = new();

        [Observable]
        private bool isDark = true;

        [Observable]
        private int speed = 80;
        [Observable]
        private int leftNum = 4;
        [Observable]
        private int rightNum = 4;

        [Observable]
        private VirtualModifiers hotKey_Start_Left = VirtualModifiers.Ctrl | VirtualModifiers.Shift;
        [Observable]
        private VirtualKeys hotKey_Start_Right = VirtualKeys.Z;

        [Observable]
        private VirtualModifiers hotKey_Stop_Left = VirtualModifiers.Ctrl | VirtualModifiers.Shift;
        [Observable]
        private VirtualKeys hotKey_Stop_Right = VirtualKeys.X;

        [Observable]
        private VirtualModifiers hotKey_ChangeMode_Left = VirtualModifiers.Ctrl | VirtualModifiers.Shift;
        [Observable]
        private VirtualKeys hotKey_ChangeMode_Right = VirtualKeys.C;

        [Observable]
        private VirtualModifiers hotKey_OpenGameVisual_Left = VirtualModifiers.Ctrl | VirtualModifiers.Shift;
        [Observable]
        private VirtualKeys hotKey_OpenGameVisual_Right = VirtualKeys.V;

        [Observable]
        private VirtualModifiers hotKey_PlusSpeed_Left = VirtualModifiers.Ctrl | VirtualModifiers.Shift;
        [Observable]
        private VirtualKeys hotKey_PlusSpeed_Right = VirtualKeys.Plus;

        [Observable]
        private VirtualModifiers hotKey_MinuSpeed_Left = VirtualModifiers.Ctrl | VirtualModifiers.Shift;
        [Observable]
        private VirtualKeys hotKey_MinuSpeed_Right = VirtualKeys.Minus;

        [Observable]
        private VirtualModifiers hotKey_PlusLeftNum_Left = VirtualModifiers.Ctrl;
        [Observable]
        private VirtualKeys hotKey_PlusLeftNum_Right = VirtualKeys.Plus;

        [Observable]
        private VirtualModifiers hotKey_MinuLeftNum_Left = VirtualModifiers.Ctrl;
        [Observable]
        private VirtualKeys hotKey_MinuLeftNum_Right = VirtualKeys.Minus;

        [Observable]
        private VirtualModifiers hotKey_PlusRightNum_Left = VirtualModifiers.Alt;
        [Observable]
        private VirtualKeys hotKey_PlusRightNum_Right = VirtualKeys.Plus;

        [Observable]
        private VirtualModifiers hotKey_MinuRightNum_Left = VirtualModifiers.Alt;
        [Observable]
        private VirtualKeys hotKey_MinuRightNum_Right = VirtualKeys.Minus;
    }

    public partial class SettingsViewModel
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        private static readonly string ConfigPath = Path.Combine(FileHelper.ConfigsFolder, "NormalConfig.json");

        public static async Task SaveFile(SettingsViewModel data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                var json = JsonSerializer.Serialize(data, jsonOptions);
                await File.WriteAllTextAsync(ConfigPath, json);
            }
            catch
            {

            }
        }

        public static async Task<SettingsViewModel> FromFile()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    return Default;
                }
                var json = await File.ReadAllTextAsync(ConfigPath);
                return JsonSerializer.Deserialize<SettingsViewModel>(json, jsonOptions)!;
            }
            catch
            {
                return Default;
            }
        }
    }
}
