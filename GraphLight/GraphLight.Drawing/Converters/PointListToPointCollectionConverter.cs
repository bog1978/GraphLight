using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GraphLight.Collections;
using GraphLight.Geometry;

namespace GraphLight.Converters
{
    public class PointListToPointCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points = value as IList<Point2D>;
            if (points == null)
                return null;
            var pointCollection = new PointCollection();
            points.Iter(x => pointCollection.Add(x.ToPoint()));
            return pointCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}