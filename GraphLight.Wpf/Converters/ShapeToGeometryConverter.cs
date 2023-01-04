using System;
using System.Globalization;
using System.Windows.Data;
using GraphLight.Graph;

namespace GraphLight.Converters
{
    internal class ShapeToGeometryConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                VertexShape.Ellipse => "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1",
                VertexShape.Rectangle => "M 0,0 L 1,0 L 1,1 L 0,1 Z",
                VertexShape.Diamond => "M 1,0 L 2,1 L 1,2 L 0,1 Z",
                _ => null,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException();
    }
}
