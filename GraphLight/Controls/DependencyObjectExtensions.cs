using System.Windows;
using System.Windows.Media;

namespace GraphLight.Controls
{
    public static class DependencyObjectExtensions
    {
        public static T GetParent<T>(this DependencyObject obj) where T : class
        {
            var parent = obj;
            do
                parent = VisualTreeHelper.GetParent(parent);
            while (parent != null && !(parent is T));
            return parent as T;
        }

        public static UIElement GetVisualTreeRoot(this UIElement dependencyObject)
        {
            var prev = dependencyObject;
            var root = dependencyObject;
            while (root != null)
            {
                prev = root;
                root = (UIElement)VisualTreeHelper.GetParent(root);
            }
            return prev;
        }
    }
}
