using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;

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
