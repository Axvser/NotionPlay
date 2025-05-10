using NotionPlay.Interfaces;

namespace NotionPlay
{
    public static class GlobalState
    {
        public static bool CanEdit { get; set; } = true;

        private static CancellationTokenSource? SimulationTokenSource;
        public static async void SubmitSimulation(ISimulable container)
        {
            CanEdit = false;
            var meta = container.GetSimulation();
            var oldSource = Interlocked.Exchange(ref SimulationTokenSource, meta.Item2);
            if (oldSource != null)
            {
                oldSource.Cancel();
                oldSource.Dispose();
            }
            await meta.Item1.Invoke();
            CanEdit = true;
        }
        public static void StopSimulation()
        {
            CanEdit = false;
            var oldSource = Interlocked.Exchange(ref SimulationTokenSource, null);
            if (oldSource != null)
            {
                oldSource.Cancel();
                oldSource.Dispose();
            }
            CanEdit = true;
        }
    }
}
