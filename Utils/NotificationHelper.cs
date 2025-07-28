using Notifications.Wpf;
using ServiceMonitor.Models;
using System.Windows.Media;

namespace ServiceMonitor.Utils
{
    public static class NotificationHelper
    {
        public static void ShowServiceStatus(string title, string message, ServiceStatus status)
        {
            var content = new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Information
            };

            var background = status switch
            {
                ServiceStatus.Ok => Brushes.LightGreen,
                ServiceStatus.Warning => Brushes.Orange,
                ServiceStatus.NotAvailable => Brushes.IndianRed,
                _ => Brushes.Gray
            };

            App.NotificationManager.Show(content, areaName: "BottomRight", onClick: () => { },
                expirationTime: TimeSpan.FromSeconds(5),
                onClose: null);
        }
    }
}
