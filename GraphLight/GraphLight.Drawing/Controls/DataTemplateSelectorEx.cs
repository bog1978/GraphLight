using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphLight.Controls
{
    public class TemplateDefinitionCollection : List<DataTemplateDefinition> { }

    public class DataTemplateSelectorEx : DataTemplateSelector
    {
        public DataTemplateSelectorEx()
        {
            TemplateDefinitions = new TemplateDefinitionCollection();
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var key = item.GetType().FullName;
            var td = TemplateDefinitions.FirstOrDefault(x => x.Key == key);
            return td.Template;
        }

        public TemplateDefinitionCollection TemplateDefinitions { get; set; }
    }
}
