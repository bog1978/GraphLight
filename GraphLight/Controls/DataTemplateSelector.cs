namespace System.Windows.Controls
{
    public abstract class DataTemplateSelector
    {
        public abstract DataTemplate SelectTemplate(object item, DependencyObject container);
    }
}