using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
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
        public string Header { get; }
        public string Message { get; }
        public Brush AccentColor { get; }
        public PackIconMaterialKind IconKind { get; set; }

        private static double _offsetY = 0;

        public ToastNotificationWindow(string title, string message, ServiceStatus status)
        {
            Header = title;
            Message = message;

            switch(status)
            {
                case ServiceStatus.Online:
                    AccentColor = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    IconKind = PackIconMaterialKind.CheckCircle;
                    break;
                case ServiceStatus.Warning:
                    AccentColor = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    IconKind = PackIconMaterialKind.AlertCircle;
                    break;
                case ServiceStatus.Offline:
                case ServiceStatus.Error:
                    AccentColor = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                    IconKind = PackIconMaterialKind.CloseCircle;
                    break;
                default:
                    AccentColor = Brushes.Gray;
                    IconKind = PackIconMaterialKind.Information;
                    break;
            }

            DataContext = this;

            // Настраиваем окно
            Topmost = true;
            ShowActivated = false; // Чтобы не забирать фокус
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;


            InitializeComponent();

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            FadeOutAndClose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PositionWindow();
            NativeMethods.SetNoActivate(new System.Windows.Interop.WindowInteropHelper(this).Handle);

            // Анимация появления
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            BeginAnimation(OpacityProperty, fadeIn);

            // Таймер автозакрытия
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
            Top = workArea.Bottom - Height - 16 - _offsetY;

            // Поднимаем смещение для следующего окна
            _offsetY += Height + 8;
        }

        private void FadeOutAndClose()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            fadeOut.Completed += (_, _) =>
            {
                _offsetY -= Height + 8;
                Close();
            };
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
