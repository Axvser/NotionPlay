using MinimalisticWPF.HotKey;
using System.Globalization;
using System.Windows.Data;

namespace NotionPlay.EditorControls.Converters
{
    public class KeyCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (uint)(VirtualKeys)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (VirtualKeys)(uint)value;
        }
    }
}
