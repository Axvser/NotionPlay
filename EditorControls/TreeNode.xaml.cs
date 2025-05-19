using MinimalisticWPF.Controls;
using NotionPlay.EditorControls.Models;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Tools;
using NotionPlay.VisualComponents;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NotionPlay.EditorControls
{
    public partial class TreeNode : StackPanel
    {
        public TreeNode()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is TreeItemViewModel viewModel)
                {
                    ViewModel = viewModel;
                    viewModel.UpdateVisual();
                }
            };
        }

        public TreeNode(TreeItemViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
            viewModel.UpdateVisual();
        }

        public TreeItemViewModel ViewModel { get; set; } = TreeItemViewModel.Empty;

        private void OpenOrCloseNode(object sender, RoutedEventArgs e)
        {
            // 仅修改ViewModel属性，通过绑定自动更新UI
            ViewModel.IsOpened = !ViewModel.IsOpened;
        }

        private void Menu_AddChild(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            var meta = NodeInfoSetter.Open(ViewModel);
            if (meta?.Item3 ?? false) return;
            if (meta is null)
            {
                NotificationBox.Confirm("⚠ 输入了不合法的节点名", "错误");
                return;
            }
            if (ViewModel.Children.Any(c => c.Header == meta.Value.Item1))
            {
                NotificationBox.Confirm("⚠ 同一层级存在同名节点", "错误");
                return;
            }
            var vm = new TreeItemViewModel()
            {
                Header = meta.Value.Item1,
                Type = meta.Value.Item2,
                Parent = ViewModel,
            };
            vm.UpdateVisual();
            ViewModel.Children.Add(vm);
            ViewModel.UpdateVisual();
            ViewModel.IsOpened = true;
        }
        private void Menu_Rename(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            if (NodeInfoSetter.Rename(out var value))
            {
                if (ViewModel.Type == TreeItemTypes.Project)
                {
                    if (SourceViewerHost?.TreeNodes.ContainsKey(value) ?? false)
                    {
                        NotificationBox.Confirm("⚠ 同一层级存在同名节点", "错误");
                        return;
                    }
                    else
                    {
                        SourceViewerHost?.TreeNodes.Remove(ViewModel.Header);
                        ViewModel.Header = value;
                        SourceViewerHost?.TreeNodes.Add(value, this);
                    }
                }
                else
                {
                    if (ViewModel.Parent.Children.Any(v => v.Header == value))
                    {
                        NotificationBox.Confirm("⚠ 同一层级存在同名节点", "错误");
                        return;
                    }
                    else
                    {
                        ViewModel.Header = value;
                    }
                }
            }
        }
        private void Menu_Delete(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            if (NotificationBox.Choose($"⚠ 确定要删除名为 : [ {ViewModel.Header} ] 的节点吗? 此操作不可撤销"))
            {
                ViewModel.Parent.Children.Remove(ViewModel);
                ViewModel.Parent.UpdateVisual();
                if (ViewModel.Type == TreeItemTypes.Project)
                {
                    SourceViewerHost?.RemoveProject(this);
                }
            }
        }
        private async void MenuItem_Snapshot(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            if (NodeInfoSetter.Snapshot(out var fileName))
            {
                var result = await TreeItemViewModel.SaveSnapshot(ViewModel, Path.Combine(FileHelper.SnapshotsFolder, FindRoot(ViewModel).Header), fileName);
                if (result)
                {
                    NotificationBox.Confirm("✔ 快照已存储", "成功 !");
                }
                else
                {
                    NotificationBox.Confirm("❌ 未能存储快照,可能是文件名存在冲突", "⚠ 失败");
                }
            }
        }
        private static TreeItemViewModel FindRoot(TreeItemViewModel viewModel)
        {
            if (viewModel.Parent is null || viewModel.Parent == TreeItemViewModel.Empty)
            {
                return viewModel;
            }
            return viewModel.Parent;
        }

        public static CancellationTokenSource? cts_ui;
        public static void StopUIUpdate()
        {
            var oldsource = Interlocked.Exchange(ref cts_ui, null);
            if (oldsource is not null)
            {
                oldsource.Cancel();
                oldsource.Dispose();
            }
        }
        private async void ShowEditor(object sender, RoutedEventArgs e)
        {
            StopSimulation();
            try
            {
                CanEdit = false;
                var source = new CancellationTokenSource();
                Interlocked.Exchange(ref cts_ui, source);
                var editors = ViewModel.Type switch
                {
                    TreeItemTypes.Paragraph => await CreateEditorAtParagraph(source),
                    TreeItemTypes.Package => await CreateEditorAtPackage(source),
                    TreeItemTypes.Project => await CreateEditorAtProject(source),
                    _ => null
                };
                if (source.IsCancellationRequested)
                {
                    CanEdit = true;
                    return;
                }
                EditorHost?.Clear();
                var counter = 0;
                if (editors is not null)
                {
                    foreach (var editor in editors)
                    {
                        if (source.IsCancellationRequested) break;
                        EditorHost?.AddParagraph(editor);
                        counter++;
                        if (counter > 3)
                        {
                            await Task.Delay(16, source.Token);
                            counter = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                CanEdit = true;
            }
        }
        private async Task<List<Paragraph>> CreateEditorAtParagraph(CancellationTokenSource cts)
        {
            if (cts.IsCancellationRequested) return [];
            return [await BuildParagraph(ViewModel, cts)];
        }
        private async Task<List<Paragraph>> CreateEditorAtPackage(CancellationTokenSource cts)
        {
            List<Paragraph> result = [];
            var counter = 0;
            foreach (TreeItemViewModel vm_paragraph in ViewModel.Children) // 遍历段落视图模型
            {
                if (cts.IsCancellationRequested) break;
                result.Add(await BuildParagraph(vm_paragraph, cts));
                counter++;
                if (counter > 4)
                {
                    await Task.Delay(16, cts.Token);
                    counter = 0;
                }
            }
            return result;
        }
        private async Task<List<Paragraph>> CreateEditorAtProject(CancellationTokenSource cts)
        {
            List<Paragraph> result = [];
            var counter = 0;
            foreach (TreeItemViewModel vm_packages in ViewModel.Children) // 遍历包视图模型
            {
                if (cts.IsCancellationRequested) break;
                foreach (TreeItemViewModel vm_paragraph in vm_packages.Children) // 遍历段落视图模型
                {
                    if (cts.IsCancellationRequested) break;
                    result.Add(await BuildParagraph(vm_paragraph, cts));
                    counter++;
                    if (counter > 4)
                    {
                        await Task.Delay(16, cts.Token);
                        counter = 0;
                    }
                }
            }
            return result;
        }
        private static async Task<Paragraph> BuildParagraph(TreeItemViewModel viewModel, CancellationTokenSource cts)
        {
            var paragraph = new Paragraph() { MusicTheory = Theory };
            var notegroups = viewModel.Notes.Select(list => list.Select(v => new SingleNote() { MusicTheory = Theory, Note = v.Note, FrequencyLevel = v.FrequencyLevel, DurationType = v.DurationType }));
            var trackcounter = 0;
            foreach (var notegroup in notegroups)
            {
                if (cts.IsCancellationRequested) break;
                var track = new Track() { MusicTheory = Theory, ParentNote = paragraph };
                foreach (var note in notegroup)
                {
                    if (cts.IsCancellationRequested) break;
                    note.ParentNote = track;
                    track.Items.Add(note);
                    trackcounter++;
                    if (trackcounter > 4)
                    {
                        await Task.Delay(16, cts.Token);
                        trackcounter = 0;
                    }
                }
                paragraph.Items.Add(track);
            }
            var notecounter = 0;
            paragraph.Saved += async () =>
            {
                var newValue = new List<List<NoteModel>>();
                foreach (Track track in paragraph.Items)
                {
                    var list = new List<NoteModel>();
                    newValue.Add(list);
                    foreach (SingleNote note in track.Items)
                    {
                        list.Add(new NoteModel()
                        {
                            Note = note.Note,
                            FrequencyLevel = note.FrequencyLevel,
                            DurationType = note.DurationType,
                        });
                        notecounter++;
                        if (notecounter > 10)
                        {
                            await Task.Delay(16, cts.Token);
                            notecounter = 0;
                        }
                    }
                }
                viewModel.Notes = newValue;
            };
            return paragraph;
        }
    }
}
