using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.ViewModels
{
    [JsonSerializable(typeof(ParagraphViewModel))]
    public class ParagraphViewModel()
    {
        public List<TrackViewModel> Tracks { get; set; } = [];
    }
}
