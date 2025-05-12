using NotionPlay.VisualComponents;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.Models
{
    [JsonSerializable(typeof(NoteMeta))]
    public class NoteMeta
    {
        public int ParagraphIndex { get; set; } = 0;
        public int TrackIndex { get; set; } = 0;
        public int NoteIndex { get; set; } = 0;
        public Notes Note { get; set; } = Notes.None;
        public FrequencyLevels FrequencyLevel { get; set; } = FrequencyLevels.None;
        public DurationTypes DurationType { get; set; } = DurationTypes.None;
        public string AbsolutePath { get; set; } = string.Empty;

        /* 功能 : 基于当前数据构建一个音符编辑视觉组件
         */
        public SingleNote BuildVisualComponent(MusicTheory config) =>
            new()
            {
                MusicTheory = config,
                Note = Note,
                FrequencyLevel = FrequencyLevel,
                DurationType = DurationType,
                VisualIndex = NoteIndex
            };

        /* 功能 : 尝试将数据Json格式保存至绝对路径中,若文件已存在则覆盖
         */
        public async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(AbsolutePath))
                throw new InvalidOperationException("Absolute path is not specified");

            var directory = Path.GetDirectoryName(AbsolutePath);
            if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            await using var fileStream = new FileStream(AbsolutePath, FileMode.Create);
            await JsonSerializer.SerializeAsync(fileStream, this);
        }

        /* List<List<List<NoteMeta>>> 最外层,表达有序段落
         * List<List<NoteMeta>> 中间层,表达单个段落内的有序音轨
         * List<NoteMeta> 最里层,表达单个音轨内的有序音符
         * 
         * 功能 : 从一个根目录开始,找到内部所有的 .json 文件,解析为 NoteMeta,
         *        解析失败则忽略而非报错;
         *        所有 NoteMeta 解析出来后,依据 ParagraphIndex / TrackIndex / NoteIndex 值,将它们重组为 List<List<List<NoteMeta>>> 结构的有序集合以表达完整的简谱
         */
        public static async Task<List<List<List<NoteMeta>>>> ParseAsync(string absoluteFolderPath)
        {
            var result = new List<List<List<NoteMeta>>>();

            if (!Directory.Exists(absoluteFolderPath))
                return result;

            var jsonFiles = Directory.EnumerateFiles(absoluteFolderPath, "*.json", SearchOption.AllDirectories);
            var notes = new List<NoteMeta>();

            foreach (var file in jsonFiles)
            {
                try
                {
                    await using var stream = File.OpenRead(file);
                    var note = await JsonSerializer.DeserializeAsync<NoteMeta>(stream);
                    if (note != null) notes.Add(note);
                }
                catch { /* 忽略解析失败 */ }
            }

            // 构建三层嵌套结构
            var paragraphGroups = notes
                .GroupBy(n => n.ParagraphIndex)
                .OrderBy(g => g.Key)
                .Select(paragraphGroup => paragraphGroup
                    .GroupBy(n => n.TrackIndex)
                    .OrderBy(g => g.Key)
                    .Select(trackGroup => trackGroup
                        .OrderBy(n => n.NoteIndex)
                        .ToList())
                    .ToList())
                .ToList();

            return paragraphGroups;
        }
    }
}
