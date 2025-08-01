using MahApps.Metro.Controls;
using System.Windows;

namespace ServiceMonitor.Views
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if(WindowState == WindowState.Minimized)
                Hide();
        }

        private void ExportServiceList(object sender, RoutedEventArgs e)
        {
            if(DataContext is ViewModels.MainViewModel vm)
            {
                vm.ExportServices();
            }
        }

        private void ImportServiceList(object sender, RoutedEventArgs e)
        {
            if(DataContext is ServiceMonitor.ViewModels.MainViewModel vm)
            {
                vm.ImportServices();
            }
        }

        private void AddService(object sender, RoutedEventArgs e)
        {
            if(DataContext is ServiceMonitor.ViewModels.MainViewModel vm)
            {
                vm.AddNewService();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if(Application.Current.ShutdownMode == ShutdownMode.OnExplicitShutdown)
            {
                if(DataContext is ServiceMonitor.ViewModels.MainViewModel vm)
                {
                    vm.SaveState();
                }
                base.OnClosing(e);
                return;
            }

            e.Cancel = true;
            Hide();
        }
    }
}
