using NotionPlay.EditorControls.Models;
using System.Text.Json.Serialization;

namespace NotionPlay.EditorControls.ViewModels
{
    [JsonSerializable(typeof(TrackViewModel))]
    public class TrackViewModel
    {
        public List<NoteMeta> Notes { get; set; } = [];
    }
}
