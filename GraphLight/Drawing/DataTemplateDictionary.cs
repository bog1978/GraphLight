using System.Collections.Generic;
using System.Windows;

namespace GraphLight.Drawing
{
    public class DataTemplateDictionary : Dictionary<string, DataTemplate>
    {
        public DataTemplate DefaultTemplate { get; set; }
    }
}