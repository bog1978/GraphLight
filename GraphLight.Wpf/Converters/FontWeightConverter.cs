using System;
using System.Globalization;
using System.Windows.Data;
using GraphLight.Graph;

namespace GraphLight.Converters
{
    internal class FontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value switch
            {
                FontWeight.Bold => System.Windows.FontWeights.Bold,
                _ => System.Windows.FontWeights.Normal,
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}