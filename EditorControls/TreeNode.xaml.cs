using MinimalisticWPF.Controls;
using NotionPlay.EditorControls.Models;
using NotionPlay.EditorControls.ViewModels;
using NotionPlay.Interfaces;
using NotionPlay.VisualComponents;
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

        private void ShowEditor(object sender, RoutedEventArgs e)
        {
            var editors = ViewModel.Type switch
            {
                TreeItemTypes.Paragraph => CreateEditorAtParagraph(),
                TreeItemTypes.Package => CreateEditorAtPackage(),
                TreeItemTypes.Project => CreateEditorAtProject(),
                _ => null
            };

            EditorHost?.Clear();
            if (editors is not null)
            {
                foreach (var editor in editors)
                {
                    EditorHost?.AddParagraph(editor);
                }
            }
        }

        private List<Paragraph> CreateEditorAtParagraph()
        {
            return [BuildParagraph(ViewModel)];
        }
        private List<Paragraph> CreateEditorAtPackage()
        {
            List<Paragraph> result = [];
            foreach (TreeItemViewModel vm_paragraph in ViewModel.Children) // 遍历段落视图模型
            {
                result.Add(BuildParagraph(vm_paragraph));
            }
            return result;
        }
        private List<Paragraph> CreateEditorAtProject()
        {
            List<Paragraph> result = [];
            foreach (TreeItemViewModel vm_packages in ViewModel.Children) // 遍历包视图模型
            {
                foreach (TreeItemViewModel vm_paragraph in vm_packages.Children) // 遍历段落视图模型
                {
                    result.Add(BuildParagraph(vm_paragraph));
                }
            }
            return result;
        }
        private static Paragraph BuildParagraph(TreeItemViewModel viewModel)
        {
            var paragraph = new Paragraph() { MusicTheory = Theory };
            var notegroups = viewModel.Notes.Select(list => list.Select(v => new SingleNote() { MusicTheory = Theory, Note = v.Note, FrequencyLevel = v.FrequencyLevel, DurationType = v.DurationType }));
            foreach (var notegroup in notegroups)
            {
                var track = new Track() { MusicTheory = Theory };
                foreach (var note in notegroup)
                {
                    track.Items.Add(note);
                }
                paragraph.Items.Add(track);
            }
            paragraph.Saved += () =>
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
                    }
                }
                viewModel.Notes = newValue;
            };
            return paragraph;
        }
    }
}
