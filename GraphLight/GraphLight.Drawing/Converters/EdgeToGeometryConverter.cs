using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GraphLight.Algorithm;
using GraphLight.Geometry;

namespace GraphLight.Converters
{
    public class EdgeToGeometryConverter : IValueConverter
    {
        public EdgeToGeometryConverter()
        {
            ArrowSize = 10;
            ArrowAngle = 15;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points = (IList<Point2D>)value;
            if (points == null || points.Count < 2)
                return null;

            points = prepare(points);
            if (EnableApproximation)
            {
                var approx = new Approximation(points);
                points = approx.GeneratePoints(20);
            }

            var grp = new GeometryGroup();
            for (var i = 0; i < points.Count - 1; i++)
                grp.Children.Add(new LineGeometry { StartPoint = points[i].ToPoint(), EndPoint = points[i + 1].ToPoint() });

            var p1 = points[points.Count - 1];
            var p2 = points[points.Count - 2];

            var v = p2 - p1;
            var k = ArrowSize / v.Len;
            var arr = p1 + v * k;
            var l1 = new LineGeometry
            {
                StartPoint = p1.ToPoint(),
                EndPoint = arr.ToPoint(),
                Transform = new RotateTransform { Angle = ArrowAngle, CenterX = p1.X, CenterY = p1.Y }
            };
            var l2 = new LineGeometry
            {
                StartPoint = p1.ToPoint(),
                EndPoint = arr.ToPoint(),
                Transform = new RotateTransform { Angle = -ArrowAngle, CenterX = p1.X, CenterY = p1.Y }
            };
            grp.Children.Add(l1);
            grp.Children.Add(l2);

            return grp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public bool EnableApproximation { get; set; }

        public double ArrowSize { get; set; }

        public double ArrowAngle { get; set; }

        private static IList<Point2D> prepare(IList<Point2D> points)
        {
            var LEN = 10;
            int i = 0;

            while (i < points.Count - 1)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                var v = p2 - p1;
                if (v.Len > LEN)
                {
                    var q = p2 - v / 2;
                    points.Insert(i + 1, q);
                }
                i++;
            }

            return points;
        }
    }
}