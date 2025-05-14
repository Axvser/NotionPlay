using System.Text.Json.Serialization;

namespace NotionPlay.VisualComponents.ViewModels
{
    [JsonSerializable(typeof(SingleNoteViewModel))]
    public partial class SingleNoteViewModel
    {
        public static SingleNoteViewModel Empty { get; private set; } = new();


    }
}
