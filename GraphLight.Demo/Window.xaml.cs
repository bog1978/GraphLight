using System.Windows;

namespace GraphLight
{
    public partial class Window
    {
        private readonly DemoViewModel _viewModel = new DemoViewModel();

        public Window()
        {
            InitializeComponent();
        }

        private void userControlLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;
        }

        private void btnOpenClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Open();
        }
    }
}
