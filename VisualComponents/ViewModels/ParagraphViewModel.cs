using MinimalisticWPF.SourceGeneratorMark;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace NotionPlay.VisualComponents.ViewModels
{
    [JsonSerializable(typeof(ParagraphViewModel))]
    public partial class ParagraphViewModel
    {
        public static ParagraphViewModel Empty { get; private set; } = new();

        [Observable]
        private ObservableCollection<TrackViewModel> tracks = [];
    }
}
