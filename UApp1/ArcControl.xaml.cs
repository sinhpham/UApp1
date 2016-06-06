using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UApp1
{
    public sealed partial class ArcControl : UserControl
    {
        public ArcControl()
        {
            this.InitializeComponent();
        }

        public double Angle1
        {
            get { return (double)GetValue(Angle1PropertyProperty); }
            set { SetValue(Angle1PropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle1Property.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Angle1PropertyProperty =
            DependencyProperty.Register("Angle1", typeof(double), typeof(ArcControl), new PropertyMetadata(0, OnAngleChanged));


        public double Angle2
        {
            get { return (double)GetValue(Angle2PropertyProperty); }
            set { SetValue(Angle2PropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle2Property.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Angle2PropertyProperty =
            DependencyProperty.Register("Angle2", typeof(double), typeof(ArcControl), new PropertyMetadata(0, OnAngleChanged));


        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ArcControl), new PropertyMetadata(0));

        public Brush FirstBrush
        {
            get { return (Brush)GetValue(FirstBrushProperty); }
            set { SetValue(FirstBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstBrushProperty =
            DependencyProperty.Register("FirstBrush", typeof(Brush), typeof(ArcControl), new PropertyMetadata(null));

        public Brush SecondBrush
        {
            get { return (Brush)GetValue(SecondBrushProperty); }
            set { SetValue(SecondBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondBrushProperty =
            DependencyProperty.Register("SecondBrush", typeof(Brush), typeof(ArcControl), new PropertyMetadata(null));

        public Brush ThirdBrush
        {
            get { return (Brush)GetValue(ThridBrushProperty); }
            set { SetValue(ThridBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThridBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThridBrushProperty =
            DependencyProperty.Register("ThirdBrush", typeof(Brush), typeof(ArcControl), new PropertyMetadata(null));




        static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ac = d as ArcControl;

            try
            {
                var a1 = ac.Angle1;
                var a2 = ac.Angle2;
            } catch (Exception)
            {
                return;
            }

            var centerP = new Point(ac.Width / 2, ac.Height / 2);
            var thickness = ac.StrokeThickness / 2;
            var sx = centerP.X - thickness;
            var sy = centerP.Y - thickness;

            var startP = new Point(centerP.X, thickness);

            var endPoint1 = CalcEndPoint(centerP, ac.Angle1, sx, sy);

            var pg1 = GenArcPathGeo(startP, endPoint1, ac.Angle1 > 180, new Size(sx, sy));
            ac._arc.Data = pg1;

            var endPoint2 = CalcEndPoint(centerP, ac.Angle1 + ac.Angle2, sx, sy);
            var pg2 = GenArcPathGeo(endPoint1, endPoint2, ac.Angle2 > 180, new Size(sx, sy));
            ac._arc2.Data = pg2;

            var pg3 = GenArcPathGeo(endPoint2, startP, 360 - ac.Angle1 - ac.Angle2 > 180, new Size(sx, sy));
            ac._arc3.Data = pg3;

            if (ac.Angle1 < 360)
            {
                var epOuterBorder = CalcEndPoint(centerP, ac.Angle1, sx + thickness - 1, sy + thickness - 1);
                var pgOuter = GenArcPathGeo(new Point(centerP.X, 1), epOuterBorder, ac.Angle1 > 180, new Size(sx + thickness - 1, sy + thickness - 1));
                ac._arcOuterBorder.Data = pgOuter;
            }
            else
            {
                var pgOuter = GenArcPathGeo(new Point(centerP.X, 1), new Point(centerP.X, ac.Height - 1), false, new Size(sx + thickness - 1, sy + thickness - 1));
                ac._arcOuterBorder.Data = pgOuter;

                var pgOuter2 = GenArcPathGeo(new Point(centerP.X, ac.Height - 1), new Point(centerP.X, 1), false, new Size(sx + thickness - 1, sy + thickness - 1));
                ac._arcInnerBorder.Data = pgOuter2;
            }
            
        }

        static PathGeometry GenArcPathGeo(Point p1, Point p2, bool isLargeArc, Size size)
        {
            var fig = new PathFigure();
            fig.StartPoint = p1;

            var arcSeg = new ArcSegment();
            arcSeg.Point = p2;
            arcSeg.IsLargeArc = isLargeArc;
            arcSeg.SweepDirection = SweepDirection.Clockwise;
            arcSeg.Size = size;

            fig.Segments.Add(arcSeg);

            var ret = new PathGeometry();
            ret.Figures.Add(fig);

            return ret;
        }

        static Point CalcEndPoint(Point center, double angle, double sizeX, double sizeY)
        {
            var a = (90.0 - angle) / 360.0 * 2 * Math.PI;
            var x = Math.Cos(a) * sizeX + center.X;
            var y = -Math.Sin(a) * sizeY + center.Y;

            return new Point(x, y);
        }
    }
}
