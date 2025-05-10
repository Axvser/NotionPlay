namespace NotionPlay.Interfaces
{
    public interface ISimulable
    {
        public (Func<Task>, CancellationTokenSource) GetSimulation();
    }
}
