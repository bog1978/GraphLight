using System;
using System.Diagnostics;
using System.Windows;

namespace GraphLight
{
    public partial class App
    {
#if SILVERLIGHT
        public App()
        {
            Startup += applicationStartup;
            InitializeComponent();
        }

        private void applicationStartup(object sender, StartupEventArgs e)
        {
            RootVisual = new MainPage();
        }
#else
        public App()
        {
            StartupUri = new Uri("Window1.xaml", UriKind.Relative);
        }
#endif
    }
}
