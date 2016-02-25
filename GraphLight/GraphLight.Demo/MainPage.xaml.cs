using System.Windows;

namespace GraphLight
{
    public partial class MainPage
    {
        private readonly DemoViewModel _viewModel = new DemoViewModel();

        public MainPage()
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
