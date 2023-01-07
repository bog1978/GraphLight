using System;
using System.Globalization;
using System.Windows.Data;
using GraphLight.Graph;

namespace GraphLight.Converters
{
    internal class TextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                TextAlignment.Left => System.Windows.HorizontalAlignment.Left,
                TextAlignment.Right => System.Windows.HorizontalAlignment.Right,
                _ => System.Windows.HorizontalAlignment.Center,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException();
    }
}
