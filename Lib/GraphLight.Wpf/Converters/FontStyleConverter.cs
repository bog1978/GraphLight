using System;
using System.Globalization;
using System.Windows.Data;
using GraphLight.Model;

namespace GraphLight.Converters
{
    internal class FontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                FontStyle.Italic => System.Windows.FontStyles.Italic,
                _ => System.Windows.FontStyles.Normal,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}