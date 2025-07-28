using ServiceMonitor.Models;
using System.Windows.Media;
using ServiceMonitor.Views;
using System.Windows;

namespace ServiceMonitor.Utils
{
    public static class Notifier
    {
        public static void Show(string message, ServiceStatus status)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string title = status switch
                {
                    ServiceStatus.Ok => "Service OK",
                    ServiceStatus.Warning => "Warning",
                    ServiceStatus.NotAvailable => "Service Down",
                    _ => "Status"
                };

                var toast = new ToastNotificationWindow(title, message, status);
                toast.Show();
            });
        }

    }
}
