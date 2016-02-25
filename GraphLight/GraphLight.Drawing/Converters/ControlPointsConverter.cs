using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using GraphLight.Geometry;
using GraphLight.Collections;

namespace GraphLight.Converters
{
    public class ControlPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ICollection<Point2D> points = (ICollection<Point2D>)value;
            if (points.Count < 2)
                return null;

            var dp = new List<Point2D>();
            dp.Add(points.First());
            points.Iter((Point2D a, Point2D b) =>
            {
                var mid = a + (b - a) / 2;
                dp.Add(b);
                dp.Add(mid);
            });
            return dp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
