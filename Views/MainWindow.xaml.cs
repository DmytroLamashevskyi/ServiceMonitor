using MahApps.Metro.Controls;
using ServiceMonitor.ViewModels;
using System.Windows;

namespace ServiceMonitor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if(WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        private void ExportServiceList(object sender, RoutedEventArgs e)
        {
            // Launch the GitHub site...
        }

        private void ImportServiceList(object sender, RoutedEventArgs e)
        {
            // deploy some CupCakes...
        }

        private void AddService(object sender, RoutedEventArgs e)
        {
            // deploy some CupCakes...
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}