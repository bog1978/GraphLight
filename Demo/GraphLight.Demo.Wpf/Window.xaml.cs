using System.Windows;

namespace GraphLight.Demo
{
    public partial class Window
    {
        public Window() => InitializeComponent();

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is DemoViewModel vm && e.NewValue is ExampleFile file)
            {
                vm.SelectedExample = file.FullName;
            }
        }
    }
}
