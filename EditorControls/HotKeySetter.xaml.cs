using MinimalisticWPF.Controls;
using MinimalisticWPF.HotKey;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using NotionPlay.EditorControls.ViewModels;
using System.Windows;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(Background), typeof(Light), ["White"])]
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Light), ["#1e1e1e"])]
    [Theme(nameof(Foreground), typeof(Dark), ["White"])]
    public partial class HotKeySetter : Window
    {
        public static HotKeySetter Instance { get; set; } = new();

        public void InitializeVisual()
        {
            UpdateHotKeyText(HK1);
            UpdateHotKeyText(HK2);
            UpdateHotKeyText(HK3);
            UpdateHotKeyText(HK4);
            UpdateHotKeyText(HK5);
            UpdateHotKeyText(HK6);
            UpdateHotKeyText(HK7);
            UpdateHotKeyText(HK8);
            UpdateHotKeyText(HK9);
            UpdateHotKeyText(HK10);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingsViewModel.SaveFile(Settings).ConfigureAwait(false);
            Instance.Hide();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        #region HotKey Event Handlers
        private void Start(object sender, HotKeyEventArgs e)
        {
            if (EditorHost is null) return;
            SubmitSimulation(EditorHost);
        }

        private void Stop(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
        }

        private void ChangeMode(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            MainWindow.Instance?.ChangeRunMode();
        }

        private void OpenGameVisuals(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            MainWindow.ChangeGameVisualState();
        }

        private void PlusSpeed(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            var newValue = Math.Clamp(Settings.Speed + 1, 1, int.MaxValue);
            Settings.Speed = newValue;
            Theory.Speed = newValue;
            MainWindow.Instance?.UpdateTheoryText();
        }

        private void MinuSpeed(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            var newValue = Math.Clamp(Settings.Speed - 1, 1, int.MaxValue);
            Settings.Speed = newValue;
            Theory.Speed = newValue;
            MainWindow.Instance?.UpdateTheoryText();
        }

        private void PlusLeftNum(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            var newValue = Math.Clamp(Settings.LeftNum + 1, 1, int.MaxValue);
            Settings.LeftNum = newValue;
            Theory.LeftNum = newValue;
            MainWindow.Instance?.UpdateTheoryText();
        }

        private void MinuLeftNum(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            var newValue = Math.Clamp(Settings.LeftNum - 1, 1, int.MaxValue);
            Settings.LeftNum = newValue;
            Theory.LeftNum = newValue;
            MainWindow.Instance?.UpdateTheoryText();
        }

        private void PlusRightNum(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            var newValue = Math.Clamp(Settings.RightNum * 2, 1, 64);
            Settings.RightNum = newValue;
            Theory.RightNum = newValue;
            MainWindow.Instance?.UpdateTheoryText();
        }

        private void MinuRightNum(object sender, HotKeyEventArgs e)
        {
            StopSimulation();
            var newValue = Math.Clamp(Settings.RightNum / 2, 1, 64);
            Settings.RightNum = newValue;
            Theory.RightNum = newValue;
            MainWindow.Instance?.UpdateTheoryText();
        }
        #endregion

        #region HotKey Update Methods
        private static void UpdateHotKeyText(HotKeyBox hotKeyBox)
        {
            var modifierNames = HotKeyHelper.GetNames([.. HotKeyHelper.GetModifiers(hotKeyBox.RecordedModifiers)]);
            var keyName = ((VirtualKeys)hotKeyBox.RecordedKey).ToString();
            var hotKeyText = string.Join(" + ", [.. modifierNames, keyName]);
            hotKeyBox.Text = hotKeyText;
        }

        private void HK1_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_Start_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK1);
        }

        private void HK2_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_Stop_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK2);
        }

        private void HK3_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_ChangeMode_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK3);
        }

        private void HK4_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_OpenGameVisual_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK4);
        }

        private void HK5_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_PlusSpeed_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK5);
        }

        private void HK6_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_MinuSpeed_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK6);
        }

        private void HK7_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_PlusLeftNum_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK7);
        }

        private void HK8_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_MinuLeftNum_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK8);
        }

        private void HK9_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_PlusRightNum_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK9);
        }

        private void HK10_ModifiersChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_MinuRightNum_Left = (VirtualModifiers)arg2;
            UpdateHotKeyText(HK10);
        }

        private void HK1_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_Start_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK1);
        }

        private void HK2_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_Stop_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK2);
        }

        private void HK3_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_ChangeMode_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK3);
        }

        private void HK4_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_OpenGameVisual_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK4);
        }

        private void HK5_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_PlusSpeed_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK5);
        }

        private void HK6_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_MinuSpeed_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK6);
        }

        private void HK7_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_PlusLeftNum_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK7);
        }

        private void HK8_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_MinuLeftNum_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK8);
        }

        private void HK9_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_PlusRightNum_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK9);
        }

        private void HK10_KeyChanged(uint arg1, uint arg2)
        {
            Settings.HotKey_MinuRightNum_Right = (VirtualKeys)arg2;
            UpdateHotKeyText(HK10);
        }
        #endregion
    }
}