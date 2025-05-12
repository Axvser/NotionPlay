using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NotionPlay.Interfaces
{
    public interface IFoldableNode
    {
        public IFoldableNode? ParentNode { get; set; }
        public ItemCollection Items { get; }
        public bool IsOpen { get; set; }
        public bool IsEnabled {  get; set; }
        public PlacementMode Placement { get; set; }
        public void Redirect();
        public void Release();
        public void OnExpanded();
        public void OnFolded();
        public void OnLocked();
        public void OnUnLocked();
    }
}
