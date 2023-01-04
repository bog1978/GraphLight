using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GraphLight.Geometry;

namespace GraphLight.Converters
{
    public class PointsToQuadraticBezierConverter : IValueConverter
    {
        public PointsToQuadraticBezierConverter()
        {
            ArrowSize = 10;
            ArrowAngle = 15;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IList<Point2D> points) || points.Count <= 1)
                return null;

            var pts = new PointCollection();
            for (var i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                pts.Add(p1.ToPoint());
                if (i >= points.Count - 1)
                    continue;
                var p2 = points[i + 1];
                pts.Add(new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2));
            }

            pts.Add(points.Last().ToPoint());


            var p11 = points[points.Count - 1];
            var p22 = points[points.Count - 2];

            var v = p22 - p11;
            var k = ArrowSize / v.Len;
            var arr = p11 + v * k;

            var grp = new GeometryGroup
            {
                Children = new GeometryCollection
                {
                    new PathGeometry
                    {
                        Figures = new PathFigureCollection
                        {
                            new PathFigure
                            {
                                StartPoint = pts.First(),
                                Segments = new PathSegmentCollection
                                {
                                    new PolyQuadraticBezierSegment { Points = pts }
                                }
                            }
                        }
                    },
                    new LineGeometry
                    {
                        StartPoint = p11.ToPoint(),
                        EndPoint = arr.ToPoint(),
                        Transform = new RotateTransform { Angle = ArrowAngle, CenterX = p11.X, CenterY = p11.Y }
                    },
                    new LineGeometry
                    {
                        StartPoint = p11.ToPoint(),
                        EndPoint = arr.ToPoint(),
                        Transform = new RotateTransform { Angle = -ArrowAngle, CenterX = p11.X, CenterY = p11.Y }
                    }
                }
            };
            return grp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

        #endregion

        public double ArrowSize { get; set; }

        public double ArrowAngle { get; set; }

    }
}