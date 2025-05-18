using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MinimalisticWPF.SourceGeneratorMark;
using MinimalisticWPF.Theme;

namespace NotionPlay.EditorControls
{
    [Theme(nameof(OBrush), typeof(Light), [nameof(Brushes.Red)])]
    [Theme(nameof(OBrush), typeof(Dark), [nameof(Brushes.Cyan)])]
    [Theme(nameof(IBrush), typeof(Light), [nameof(Brushes.Violet)])]
    [Theme(nameof(IBrush), typeof(Dark), [nameof(Brushes.White)])]
    [MonoBehaviour(16)]
    public partial class LoadingPage : UserControl
    {
        public Transform OChildTransform
        {
            get { return (Transform)GetValue(OChildTransformProperty); }
            set { SetValue(OChildTransformProperty, value); }
        }
        public static readonly DependencyProperty OChildTransformProperty =
            DependencyProperty.Register("OChildTransform", typeof(Transform), typeof(LoadingPage), new PropertyMetadata(Transform.Identity));
        public Transform IChildTransform
        {
            get { return (Transform)GetValue(IChildTransformProperty); }
            set { SetValue(IChildTransformProperty, value); }
        }
        public static readonly DependencyProperty IChildTransformProperty =
            DependencyProperty.Register("IChildTransform", typeof(Transform), typeof(LoadingPage), new PropertyMetadata(Transform.Identity));
        public Brush OBrush
        {
            get { return (Brush)GetValue(OBrushProperty); }
            set { SetValue(OBrushProperty, value); }
        }
        public static readonly DependencyProperty OBrushProperty =
            DependencyProperty.Register("OBrush", typeof(Brush), typeof(LoadingPage), new PropertyMetadata(Brushes.Transparent));
        public Brush IBrush
        {
            get { return (Brush)GetValue(IBrushProperty); }
            set { SetValue(IBrushProperty, value); }
        }
        public static readonly DependencyProperty IBrushProperty =
            DependencyProperty.Register("IBrush", typeof(Brush), typeof(LoadingPage), new PropertyMetadata(Brushes.Transparent));

        private readonly RotateTransform rotateO = new(0, 0, 0);
        private readonly RotateTransform rotateI = new(0, 0, 0);

        private double opacitydelta = 0.01;
        private double textopacitydirection = 1;

        partial void Awake()
        {
            SizeChanged += (s, e) =>
            {
                rotateO.CenterX = ActualHeight / 2;
                rotateO.CenterY = ActualHeight / 2;
                rotateI.CenterX = ActualHeight / 2;
                rotateI.CenterY = ActualHeight / 2;
                FontSize = ActualHeight / 10;
            };
            Loaded += (s, e) =>
            {
                rotateO.CenterX = ActualHeight / 2;
                rotateO.CenterY = ActualHeight / 2;
                rotateI.CenterX = ActualHeight / 2;
                rotateI.CenterY = ActualHeight / 2;
                FontSize = ActualHeight / 10;
                OChildTransform = rotateO;
                IChildTransform = rotateI;
                CanMonoBehaviour = false;
            };
        }

        partial void Update()
        {
            rotateO.Angle += 1;
            rotateI.Angle -= 4;
            TextView.Opacity += textopacitydirection * opacitydelta;
        }

        partial void LateUpdate()
        {
            textopacitydirection = textopacitydirection > 0 ?
                (TextView.Opacity >= 1 ? -1 : 1)
                :
                (TextView.Opacity <= 0 ? 1 : -1);
        }
    }
}
