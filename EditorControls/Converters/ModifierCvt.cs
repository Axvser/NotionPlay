﻿using MinimalisticWPF.HotKey;
using System.Globalization;
using System.Windows.Data;

namespace NotionPlay.EditorControls.Converters
{
    public class ModifierCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (uint)(VirtualModifiers)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var modifiers = HotKeyHelper.GetModifiers((uint)value);
            var result = VirtualModifiers.None;
            foreach (var modifier in modifiers)
            {
                result |= modifier;
            }
            return result;
        }
    }
}
