using System.Windows;
using System.Windows.Controls;

namespace GraphLight.Controls
{
    public static class ContainerBindings
    {
        public static readonly DependencyProperty BindingCollectionProperty = DependencyProperty.RegisterAttached(
            "BindingCollection", typeof(ContainerBindingCollection), typeof(ContainerBindings), new PropertyMetadata(onBindingCollectionPropertyChanged));

        public static void SetBindingCollection(DependencyObject element, ContainerBindingCollection value)
        {
            element.SetValue(BindingCollectionProperty, value);
        }

        public static ContainerBindingCollection GetBindingCollection(DependencyObject element)
        {
            return (ContainerBindingCollection)element.GetValue(BindingCollectionProperty);
        }

        private static void onBindingCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as FrameworkElement;
            if (ctrl == null)
                return;
            var oldValue = e.OldValue as ContainerBindingCollection;
            var newValue = e.NewValue as ContainerBindingCollection;

            if (newValue != null)
            {
                var container = ctrl.GetParent<ContentPresenter>();
                if (container != null)
                    aaa(ctrl);
                else
                {
                    RoutedEventHandler handler = null;
                    handler = (s1,e1) =>
                        {
                            var element = (FrameworkElement) s1;
                            element.Loaded -= handler;
                            aaa(element);
                        };
                    ctrl.Loaded += handler;
                }
            }
        }

        private static void aaa(FrameworkElement ctrl)
        {
            var bindings = GetBindingCollection(ctrl);
            var container = ctrl.GetParent<ContentPresenter>();
            foreach (var containerBinding in bindings)
            {
                var dp = containerBinding.GetDependencyProperty(typeof(Canvas));
                if (dp != null)
                    container.SetBinding(dp, containerBinding.Binding);
            }
        }
    }
}