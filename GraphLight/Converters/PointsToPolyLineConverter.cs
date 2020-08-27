using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using GraphLight.Geometry;

namespace GraphLight.Converters
{
    public class PointsToPolyLineConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points2D = (IList<Point2D>) value;
            if (points2D == null || points2D.Count < 2)
                return null;

            var points = new PointCollection();
            foreach (var p1 in points2D)
                points.Add(p1.ToPoint());

            var geom = new PathGeometry
                {
                    Figures = new PathFigureCollection
                        {
                            new PathFigure
                                {
                                    StartPoint = points.First(),
                                    Segments = new PathSegmentCollection {new PolyLineSegment {Points = points}}
                                }
                        }
                };
            return geom;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

        #endregion
    }
}