    using ServiceMonitor.Models;
    using ServiceMonitor.Views;
    using System.Collections.Generic;
    using System.Windows;

    namespace ServiceMonitor.Utils
    {
        public static class Notifier
        {
            // Активные тосты, отсортированы по вертикальному смещению (снизу вверх)
            private static readonly List<ToastNotificationWindow> _activeToasts = new();

            // Максимальное количество одновременно отображаемых тостов (можно настроить)
            private const int _maxToasts = 5;

            public static void Show(string serviceName, string message, ServiceStatus status)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Если много тостов уже, можно игнорировать или удалить самый старый
                    if(_activeToasts.Count >= _maxToasts)
                    {
                        // Можно удалить первый (самый верхний) или просто не показывать новый
                        var first = _activeToasts[0];
                        first.Close();
                        // Или просто return; чтобы игнорировать
                    }

                    var toast = new ToastNotificationWindow(serviceName, message, status);
                    toast.Closed += Toast_Closed;

                    _activeToasts.Add(toast);
                    UpdateToastPositions();

                    toast.Show();
                });
            }

            private static void Toast_Closed(object? sender, System.EventArgs e)
            {
                if(sender is ToastNotificationWindow toast)
                {
                    toast.Closed -= Toast_Closed;
                    _activeToasts.Remove(toast);
                    UpdateToastPositions();
                }
            }

            private const double _toastHeight = 100;
            private const double _bottomMargin = 16;
            private const double _horizontalMargin = 16;
            private const double _verticalSpacing = 8;

            private static void UpdateToastPositions()
            {
                var workArea = SystemParameters.WorkArea;
                double currentOffset = _bottomMargin;

                for(int i = _activeToasts.Count - 1; i >= 0; i--)
                {
                    var toast = _activeToasts[i];
                    toast.Left = workArea.Right - toast.Width - _horizontalMargin;
                    toast.Top = workArea.Bottom - _toastHeight - currentOffset;
                    currentOffset += _toastHeight + _verticalSpacing;
                }
            }
        }
    }
