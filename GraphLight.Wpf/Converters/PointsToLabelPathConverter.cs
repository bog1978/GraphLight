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
    public class PointsToLabelPathConverter : IValueConverter
    {
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

            var grp = new PathFigure
            {
                StartPoint = pts.First(),
                Segments = new PathSegmentCollection
                {
                    new PolyQuadraticBezierSegment { Points = pts }
                }
            };
            return grp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}