using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpotLibrary.Pie
{
    public class PieSector : Shape
    {
        #region Center

        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(PieSector), 
                new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Start Angle

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }
        
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(PieSector), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region End Angle

        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }
        
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(PieSector),
                new FrameworkPropertyMetadata(Math.PI / 2.0, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Radius

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(PieSector),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Small Angle

        public bool SmallAngle
        {
            get { return (bool)GetValue(SmallAngleProperty); }
            set { SetValue(SmallAngleProperty, value); }
        }

        public static readonly DependencyProperty SmallAngleProperty =
            DependencyProperty.Register("SmallAngle", typeof(bool), typeof(PieSector),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        static PieSector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieSector), new FrameworkPropertyMetadata(typeof(PieSector)));
        }

        protected override Geometry DefiningGeometry
        {
            get
            {

                var a0 = StartAngle < 0 ? StartAngle + 2 * Math.PI : StartAngle;
                var a1 = EndAngle < 0 ? EndAngle + 2 * Math.PI : EndAngle;

                if (a1 < a0)
                {
                    a1 += Math.PI * 2;
                }

                SweepDirection d = SweepDirection.Counterclockwise;
                bool large;

                if (SmallAngle)
                {
                    large = false;
                    double t = a1;
                    if ((a1 - a0) > Math.PI)
                    {
                        d = SweepDirection.Counterclockwise;
                    }
                    else
                    {
                        d = SweepDirection.Clockwise;
                    }


                }
                else {
                    large = (Math.Abs(a1 - a0) < Math.PI);
                }

                Point p0 = Center + new Vector(Math.Cos(a0), Math.Sin(a0)) * Radius;
                Point p1 = Center + new Vector(Math.Cos(a1), Math.Sin(a1)) * Radius;


                List<PathSegment> segments = new List<PathSegment>(1);
                segments.Add(new ArcSegment(p1, new Size(Radius, Radius), 0.0, large, d, true));

                List<PathFigure> figures = new List<PathFigure>(1);
                PathFigure pf = new PathFigure(p0, segments, true);
                pf.IsClosed = false;
                figures.Add(pf);

                Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);
                return g;
            }
        }
    }
}
