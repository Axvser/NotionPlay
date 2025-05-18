using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(OBrush), typeof(Light), [nameof(Brushes.Red)])]
    [Theme(nameof(OBrush), typeof(Dark), [nameof(Brushes.Cyan)])]
    [Theme(nameof(IBrush), typeof(Light), [nameof(Brushes.Black)])]
    [Theme(nameof(IBrush), typeof(Dark), [nameof(Brushes.White)])]
    public partial class ProgressBar : UserControl
    {
        public Brush OBrush
        {
            get { return (Brush)GetValue(OBrushProperty); }
            set { SetValue(OBrushProperty, value); }
        }
        public static readonly DependencyProperty OBrushProperty =
            DependencyProperty.Register("OBrush", typeof(Brush), typeof(ProgressBar), new PropertyMetadata(Brushes.Transparent));
        public Brush IBrush
        {
            get { return (Brush)GetValue(IBrushProperty); }
            set { SetValue(IBrushProperty, value); }
        }
        public static readonly DependencyProperty IBrushProperty =
            DependencyProperty.Register("IBrush", typeof(Brush), typeof(ProgressBar), new PropertyMetadata(Brushes.Transparent));
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(ProgressBar), new PropertyMetadata(0d));

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressBar), new PropertyMetadata(0d, (dp, e) =>
            {
                if (dp is ProgressBar bar)
                {
                    bar.EndAngle = (double)e.NewValue * 360;
                    bar.TextView.Text = $"{(int)(bar.Progress * 100)}%";
                }
            }));
    }
}
