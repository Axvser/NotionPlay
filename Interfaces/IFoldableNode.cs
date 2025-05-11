using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NotionPlay.Interfaces
{
    public interface IFoldableNode
    {
        public IFoldableNode? ParentNode { get; set; }
        public ItemCollection Items { get; }
        public bool IsOpen { get; set; }
        public PlacementMode Placement { get; set; }
    }
}
