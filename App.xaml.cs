using Hardcodet.Wpf.TaskbarNotification;
using Notifications.Wpf;
using ServiceMonitor.Models;
using ServiceMonitor.Utils;
using ServiceMonitor.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ServiceMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static NotificationManager NotificationManager { get; } = new NotificationManager();
        private TaskbarIcon? _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");

            MainWindow = new MainWindow();
            MainWindow.Hide();
        }

        // Обработчик "Открыть"
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            if(MainWindow == null)
            {
                MainWindow = new Views.MainWindow();
            }

            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Current.MainWindow;

            if(mainWindow == null)
            {
                mainWindow = new MainWindow();
                Current.MainWindow = mainWindow;
            }

            if(!mainWindow.IsVisible)
                mainWindow.Show();

            if(mainWindow.WindowState == WindowState.Minimized)
                mainWindow.WindowState = WindowState.Normal;

            mainWindow.Activate();
        }


        // Обработчик "Выход"
        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            _trayIcon?.Dispose();
            Shutdown();
        }
    }

}
