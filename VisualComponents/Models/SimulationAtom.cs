namespace NotionPlay.VisualComponents.Models
{
    /// <summary>
    /// ✨ 原子化按键模拟操作,这对于维系多音轨同时播放时的时间同步率至关重要
    /// </summary>
    public class SimulationAtom()
    {
        public event Action? KeyDowns;
        public int Span = 0;
        public event Action? KeyUps;
        public void Run()
        {
            KeyDowns?.Invoke();
        }
        public void Release()
        {
            KeyUps?.Invoke();
        }
    }
}
