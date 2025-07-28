using ServiceMonitor.Models;
using System.Windows.Media;
using ServiceMonitor.Views;
using System.Windows;

namespace ServiceMonitor.Utils
{
    public static class Notifier
    {
        public static void Show(string ServiceName, string message, ServiceStatus status)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new ToastNotificationWindow(ServiceName, message, status);
                toast.Show();
            });
        }

    }
}
