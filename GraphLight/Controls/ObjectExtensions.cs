using System.Windows;

namespace GraphLight.Controls
{
    public static class ObjectExtensions
    {
        public static T GetDataContext<T>(this object obj) where T : class =>
            obj is FrameworkElement element
                ? element.DataContext as T
                : null;
    }
}