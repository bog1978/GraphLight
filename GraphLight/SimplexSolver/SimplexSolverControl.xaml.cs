using System.Windows.Input;

namespace WpfSimplexSolver
{
    /// <summary>
    /// Interaction logic for SimplexSolverControl.xaml
    /// </summary>
    public partial class SimplexSolverControl
    {
        public SimplexSolverControl()
        {
            InitializeComponent();
        }

        private SimplexSolverViewModel _vm;

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void onBtnAddUnknownClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm.AddUnknown();
        }

        private void onBtnAddRestrictionClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm.AddRestriction();
        }

        private void onBtnSolveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm.Solve();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm = (SimplexSolverViewModel) grid.DataContext;
        }
    }
}
