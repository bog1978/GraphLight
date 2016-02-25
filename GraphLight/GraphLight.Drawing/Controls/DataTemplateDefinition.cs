using System.Windows;
using System.Windows.Markup;

namespace GraphLight.Controls
{
    [ContentProperty("Template")]
    public class DataTemplateDefinition : DependencyObject
    {
        public string Key {get;set;}
        public DataTemplate Template { get; set; }
    }
}