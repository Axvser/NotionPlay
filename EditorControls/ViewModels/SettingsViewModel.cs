using Microsoft.Win32;
using MinimalisticWPF.Controls;
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
        private int speed = 80;
        [Observable]
        private int leftNum = 4;
        [Observable]
        private int rightNum = 4;
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
