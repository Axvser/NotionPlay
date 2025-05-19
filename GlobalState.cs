using NotionPlay.EditorControls;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Interfaces;
using NotionPlay.VisualComponents;
using NotionPlay.VisualComponents.Models;
using System.Windows;

namespace NotionPlay
{
    public static class GlobalState
    {
        public static bool CanEdit { get; set; } = true;

        public static bool CanSimulate { get; set; } = false;
        public static bool CanPreview { get; set; } = true;
        public static bool CanHightLight { get; set; } = true;
        
        public static bool CanTheorySetter { get; set; } = false;

        public static MusicTheory Theory { get; set; } = new();
        public static SourceViewer? SourceViewerHost { get; set; }
        public static NumberedMusicalNotationEditor? EditorHost { get; set; }
        public static GameVisual? GameVisualHost { get; set; }
        public static Style? Default_NoteStyle { get; private set; } = Application.Current.TryFindResource("nicebutton2") as Style;

        public static SettingsViewModel Settings { get; set; } = SettingsViewModel.Default;

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
            TreeNode.StopUIUpdate();
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
