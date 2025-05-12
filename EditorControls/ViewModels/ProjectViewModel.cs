using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.ViewModels
{
    [JsonSerializable(typeof(ProjectViewModel))]
    public class ProjectViewModel
    {
        public List<ParagraphViewModel> Paragraphs { get; set; } = [];
    }
}
