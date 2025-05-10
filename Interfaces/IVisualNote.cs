using NotionPlay.VisualComponents;
using NotionPlay.VisualComponents.Enums;

namespace NotionPlay.Interfaces
{
    public interface IVisualNote
    {
        public MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; }
        public VisualTypes VisualType { get; set; }
    }
}
