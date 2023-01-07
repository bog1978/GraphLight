using System;
using System.Globalization;
using System.Windows.Data;
using GraphLight.Graph;

namespace GraphLight.Converters
{
    internal class TextWrappingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                TextWrapping.Wrap => System.Windows.TextWrapping.Wrap,
                _ => System.Windows.TextWrapping.NoWrap,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException();
    }
}