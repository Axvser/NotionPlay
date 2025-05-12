using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.Windows.Controls;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    [Hover([nameof(Background)])]
    public partial class FileControl : UserControl, IVisualNote
    {
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; }
        public virtual VisualTypes VisualType { get; protected set; } = VisualTypes.None;
    }
}
