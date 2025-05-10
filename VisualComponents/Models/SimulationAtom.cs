namespace NotionPlay.VisualComponents.Models
{
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
