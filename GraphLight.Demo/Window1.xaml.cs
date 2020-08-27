using System.Windows;

namespace GraphLight
{
    public partial class Window1
    {
        private readonly DemoViewModel _viewModel = new DemoViewModel();

        public Window1()
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
