using System.Collections.Generic;
using System.Windows;

namespace GraphLight.Drawing
{
    public class DataTemplateDictionary : Dictionary<string, DataTemplate>
    {
        public DataTemplate? DefaultTemplate { get; set; }

        public DataTemplate? FindTemplate(string? key) =>
            key switch
            {
                null => null,
                _ => TryGetValue(key, out var template)
                    ? template
                    : DefaultTemplate,
            };
    }
}