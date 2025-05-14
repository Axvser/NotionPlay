using MinimalisticWPF.SourceGeneratorMark;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace NotionPlay.VisualComponents.ViewModels
{
    [JsonSerializable(typeof(TrackViewModel))]
    public partial class TrackViewModel
    {
        public static TrackViewModel Empty { get; private set; } = new();

        [Observable]
        private ObservableCollection<SingleNoteViewModel> singleNotes = [];
    }
}
