using ControlzEx.Standard;
using MahApps.Metro.Controls;
using ServiceMonitor.Models;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ServiceMonitor.Views
{
    public partial class ToastNotificationWindow : MetroWindow
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public Brush AccentColor { get; set; } = Brushes.Gray;
        public string IconPath { get; set; } = "";

        public ToastNotificationWindow(string title, string message, ServiceStatus status)
        {
            InitializeComponent();

            Title = title;
            Message = message;

            switch(status)
            {
                case ServiceStatus.Ok:
                    AccentColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
                    IconPath = "Assets/ok.png";
                    break;
                case ServiceStatus.Warning:
                    AccentColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
                    IconPath = "Assets/warning.png";
                    break;
                case ServiceStatus.NotAvailable:
                    AccentColor = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
                    IconPath = "Assets/error.png";
                    break;
                default:
                    AccentColor = Brushes.Gray;
                    IconPath = "Assets/info.png";
                    break;
            }

            DataContext = this;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            FadeOutAndClose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PositionWindow();

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            BeginAnimation(OpacityProperty, fadeIn);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                FadeOutAndClose();
            };
            timer.Start();
        }

        private void PositionWindow()
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 16;
            Top = workArea.Bottom - Height - 16;
        }

        private void FadeOutAndClose()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            fadeOut.Completed += (_, _) => Close();
            BeginAnimation(OpacityProperty, fadeOut);
        }


    }

    public static class NativeMethods
    {
        private const int _gWL_EXSTYLE = -20;
        private const int _wS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        public static void SetNoActivate(IntPtr hwnd)
        {
            int exStyle = GetWindowLong(hwnd, _gWL_EXSTYLE);
            SetWindowLong(hwnd, _gWL_EXSTYLE, exStyle | _wS_EX_NOACTIVATE);
        }
    }
}
