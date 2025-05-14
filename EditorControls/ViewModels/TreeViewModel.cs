using Microsoft.Win32;
using MinimalisticWPF.Controls;
using MinimalisticWPF.SourceGeneratorMark;
using NotionPlay.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace NotionPlay.EditorControls.ViewModels
{
    public enum TreeItemTypes : int
    {
        None = 0,
        Project = 1,
        Paragraph = 2,
        Track = 3,
        Package = 4,
    }

    [JsonSerializable(typeof(TreeItemViewModel))]
    public partial class TreeItemViewModel
    {
        public static TreeItemViewModel Empty { get; private set; } = new();

        [Constructor]
        private void SetLocal()
        {
            Local = this;
        }
        
        [Observable]
        private TreeItemViewModel parent = Empty;
        [Observable]
        private TreeItemViewModel local;
        [Observable]
        private string stateIcon = ClosedSVG;
        [Observable]
        private string typeIcon = string.Empty;
        [Observable]
        private string header = string.Empty;
        [Observable(Validations.None)]
        private bool isOpened = false;
        [Observable(Validations.None)]
        private TreeItemTypes type = TreeItemTypes.None;
        [Observable(Validations.None)]
        private Visibility itemsVisibility = Visibility.Collapsed;
        [Observable(Validations.None)]
        private bool isContextMenuEnabled = false;
        [Observable(Validations.None)]
        private int paragraphIndex = 0;
        [Observable]
        private ObservableCollection<TreeItemViewModel> children = [];
    }

    public partial class TreeItemViewModel
    {
        partial void OnIsOpenedChanged(bool oldValue, bool newValue)
        {
            UpdateVisual();
        }
        partial void OnTypeChanged(TreeItemTypes oldValue, TreeItemTypes newValue)
        {
            TypeIcon = newValue switch
            {
                TreeItemTypes.Project => ProjectSVG,
                TreeItemTypes.Paragraph => ParagraphSVG,
                TreeItemTypes.Track => TrackSVG,
                TreeItemTypes.Package => PackageSVG,
                _ => string.Empty
            };
            IsContextMenuEnabled = newValue switch
            {
                TreeItemTypes.Project => true,
                TreeItemTypes.Paragraph => true,
                TreeItemTypes.Package => true,
                _ => false
            };
        }
        public void UpdateVisual()
        {
            StateIcon = Children.Count > 0 ? (IsOpened ? OpenedSVG : ClosedSVG) : string.Empty;
            ItemsVisibility = IsOpened ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public partial class TreeItemViewModel
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public static async Task Save(TreeItemViewModel itemToSave)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "保存项目为JSON文件",
                DefaultExt = ".json",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await using var fileStream = saveFileDialog.OpenFile();
                    await JsonSerializer.SerializeAsync(fileStream, itemToSave, jsonOptions);
                    NotificationBox.Confirm("✔ 项目保存成功", "成功");
                }
                catch (Exception)
                {
                    NotificationBox.Confirm("❌ 项目保存失败", "失败");
                }
            }
        }
        public static async Task<TreeItemViewModel> FromFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "选择 JSON 文件",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await using var fileStream = openFileDialog.OpenFile();
                    return await JsonSerializer.DeserializeAsync<TreeItemViewModel>(fileStream, jsonOptions) ?? Empty;
                }
                catch (Exception)
                {
                    NotificationBox.Confirm("⚠ 无法将指定的Json文件转换为项目", "错误");
                }
            }

            return Empty;
        }
    }

    public partial class TreeItemViewModel
    {
        private const string OpenedSVG = "M891.741028 886.32859591l-503.48205604 5.41243209 503.48205606-498.06962393z";
        private const string ClosedSVG = "M93.37191 57.154573 930.62809 540.635608 93.37191 1024Z";

        private const string ProjectSVG = "m256 34.347l192 110.851v221.703L256 477.752L64 366.901V145.198zM106.666 192.001v150.266l128 73.9V265.902zm298.667.001l-128 73.9v150.265l128-73.9zM256 83.614l-125.867 72.67L256 228.952l125.867-72.67z";
        private const string ParagraphSVG = "M672.526222 25.088h565.361778v140.060444H672.526222zM672.526222 301.980444h317.838222v140.060445H672.526222zM672.526222 578.872889h494.648889v140.060444h-494.648889z M1237.902222 0.312889H672.526222a25.031111 25.031111 0 0 0-25.002666 24.988444v140.074667c0 13.795556 11.207111 24.988444 25.002666 24.988444H1237.902222a25.031111 25.031111 0 0 0 25.016889-24.988444V25.301333A25.031111 25.031111 0 0 0 1237.902222 0.312889z m-25.230222 139.847111h-515.128889V50.304h515.128889v89.856zM672.526222 467.043556h317.838222a24.746667 24.746667 0 0 0 25.016889-25.002667v-140.060445a25.031111 25.031111 0 0 0-25.031111-25.002666H672.540444a25.031111 25.031111 0 0 0-25.002666 25.002666v140.060445c0 13.795556 11.207111 25.002667 25.002666 25.002667z m25.031111-139.847112h267.804445v89.856h-267.804445v-89.856zM1167.175111 553.884444h-494.648889a25.031111 25.031111 0 0 0-25.002666 24.988445v140.074667c0 13.795556 11.207111 24.988444 25.002666 24.988444h494.648889a25.031111 25.031111 0 0 0 25.016889-24.988444v-140.074667a25.031111 25.031111 0 0 0-25.016889-24.988445z m-25.230222 139.847112H697.543111V603.875556h444.401778v89.856zM919.637333 831.004444h-246.897777a25.031111 25.031111 0 0 0-25.002667 24.988445c0 13.795556 11.207111 24.988444 25.016889 24.988444h221.866666v89.856h-221.866666a25.031111 25.031111 0 0 0-25.031111 25.002667c0 13.795556 11.235556 25.002667 25.031111 25.002667h247.096889c13.582222 0 25.031111-11.420444 24.803555-24.789334v-140.060444a25.031111 25.031111 0 0 0-25.016889-25.002667zM410.552889 489.671111L187.804444 335.160889a24.888889 24.888889 0 0 0-25.870222-1.706667c-8.405333 4.295111-13.582222 12.913778-13.582222 22.186667v59.463111H25.016889A25.031111 25.031111 0 0 0 0 440.106667v139.207111a25.031111 25.031111 0 0 0 50.033778 0v-114.204445h123.335111a25.031111 25.031111 0 0 0 25.016889-25.002666v-36.636445l153.955555 106.88-153.969777 106.88v-37.916444a25.031111 25.031111 0 0 0-50.019556 0v85.973333a25.116444 25.116444 0 0 0 25.002667 25.002667c5.176889 0 10.140444-1.507556 14.222222-4.522667l222.976-155.164444c6.684444-4.736 10.780444-12.273778 10.780444-20.465778 0-8.192-4.096-15.729778-10.780444-20.48zM527.203556 3.754667a25.031111 25.031111 0 0 0-25.016889 25.002666v963.413334c0 13.795556 11.207111 25.002667 25.016889 25.002666 13.795556 0 25.016889-11.207111 25.016888-25.216V28.743111a25.031111 25.031111 0 0 0-25.031111-24.988444z";
        private const string TrackSVG = "M3 6v8h5.635L12 19.908V27h8v-8h-6.217l-2.845-5H11v-3h10v3h8V6h-8v3H11V6zm2 2h4v4H5zm18 0h4v4h-4zm-8.582 13H18v4h-4v-3.762z";
        private const string PackageSVG = "M132.266667 358.4l17.066666 8.533333 345.6 204.8 379.733334-209.066666 21.333333-8.533334 29.866667-17.066666c12.8-8.533333 12.8-29.866667 0-38.4l-29.866667-12.8-25.6-12.8-371.2-200.533334c-8.533333-4.266667-12.8-4.266667-21.333333 0L149.333333 268.8l-17.066666 12.8-34.133334 17.066667c-12.8 8.533333-12.8 29.866667 0 38.4l34.133334 21.333333z m362.666666-196.266667l290.133334 157.866667-290.133334 157.866667-264.533333-157.866667 264.533333-157.866667z M925.866667 512l-29.866667-12.8-25.6-12.8-379.733333 209.066667-345.6-209.066667-17.066667 8.533333-29.866667 17.066667c-12.8 8.533333-12.8 29.866667 0 38.4l34.133334 21.333333 17.066666 8.533334 345.6 204.8 379.733334-209.066667 25.6-12.8 29.866666-17.066667c12.8-4.266667 12.8-25.6-4.266666-34.133333z M925.866667 682.666667l-29.866667-12.8-25.6-12.8-379.733333 209.066666-345.6-209.066666-17.066667 8.533333-29.866667 17.066667c-12.8 8.533333-12.8 29.866667 0 38.4l34.133334 21.333333 17.066666 8.533333 345.6 204.8 379.733334-209.066666 25.6-12.8 29.866666-17.066667c12.8-4.266667 12.8-25.6-4.266666-34.133333z";
    }
}
