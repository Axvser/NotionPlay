using MinimalisticWPF.SourceGeneratorMark;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace NotionPlay.VisualComponents.ViewModels
{
    [JsonSerializable(typeof(PackageViewModel))]
    public partial class PackageViewModel
    {
        public static PackageViewModel Empty { get; private set; } = new();

        [Observable]
        private ObservableCollection<ParagraphViewModel> paragraphs = [];
    }
}
