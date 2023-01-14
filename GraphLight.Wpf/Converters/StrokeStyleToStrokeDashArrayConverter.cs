using System;
using System.Globalization;
using System.Windows.Data;
using GraphLight.Model;

namespace GraphLight.Converters
{
    internal class StrokeStyleToStrokeDashArrayConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                StrokeStyle.Dash => "4 2",
                StrokeStyle.DashDot => "4 2 1 2",
                StrokeStyle.Dot => "1 2",
                StrokeStyle.Solid => null,
                _ => null,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}