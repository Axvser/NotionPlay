using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.Interfaces;
using NotionPlay.VisualComponents.Enums;
using NotionPlay.VisualComponents.Models;
using System.Windows;

namespace NotionPlay.EditorControls
{
    public enum FileTypes : int
    {
        Track = 1,
        Paragraph = 2,
        Project = 3,
        None = 0
    }

    public partial class FileControl : NodeControl, IVisualNote
    {
        public required MusicTheory MusicTheory { get; set; }
        public IVisualNote? ParentNote { get; set; }
        public int VisualIndex { get; set; }
        public virtual VisualTypes VisualType { get; protected set; } = VisualTypes.None;

        public FileTypes FileType
        {
            get { return (FileTypes)GetValue(FileTypeProperty); }
            set { SetValue(FileTypeProperty, value); }
        }
        public static readonly DependencyProperty FileTypeProperty =
            DependencyProperty.Register("FileType", typeof(FileTypes), typeof(FileControl), new PropertyMetadata(FileTypes.None, (dp, e) =>
            {
                if (dp is FileControl control)
                {
                    control.FileTypeSymbol = (int)e.NewValue switch
                    {
                        1 => TrackSymbol,
                        2 => ParagraphSymbol,
                        3 => SheetMusicSymbol,
                        _ => string.Empty,
                    };
                }
            }));

        public string FileTypeSymbol
        {
            get { return (string)GetValue(FileTypeSymbolProperty); }
            set { SetValue(FileTypeSymbolProperty, value); }
        }
        public static readonly DependencyProperty FileTypeSymbolProperty =
            DependencyProperty.Register("FileTypeSymbol", typeof(string), typeof(FileControl), new PropertyMetadata(string.Empty));
    }

    public partial class FileControl
    {
        const string TrackSymbol = "M4 20q-.825 0-1.412-.587T2 18V6q0-.825.588-1.412T4 4h6l2 2h8q.825 0 1.413.588T22 8v10q0 .825-.587 1.413T20 20z";
        const string ParagraphSymbol = "M10 2a1 1 0 0 1 1 1v4a1 1 0 0 1-1 1H8v2h5V9a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v4a1 1 0 0 1-1 1h-6a1 1 0 0 1-1-1v-1H8v6h5v-1a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v4a1 1 0 0 1-1 1h-6a1 1 0 0 1-1-1v-1H7a1 1 0 0 1-1-1V8H4a1 1 0 0 1-1-1V3a1 1 0 0 1 1-1zm9 16h-4v2h4zm0-8h-4v2h4zM9 4H5v2h4z";
        const string SheetMusicSymbol = "M10 8v11.023A4.986 4.986 0 1 0 11.9 24h.1V11.6l16-3.2v8.623A4.986 4.986 0 1 0 29.9 22h.1V4Z";
    }
}
