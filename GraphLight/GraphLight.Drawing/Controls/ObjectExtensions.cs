using System.Windows;

namespace GraphLight.Controls
{
    public static class ObjectExtensions
    {
        public static T GetDataContext<T>(this object obj)
            where T : class
        {
            var element = obj as FrameworkElement;
            if (element == null)
                return null;
            return element.DataContext as T;
        }
    }
}