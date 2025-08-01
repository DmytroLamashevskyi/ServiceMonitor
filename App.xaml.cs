using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Logging;
using ServiceMonitor.Core;
using ServiceMonitor.Models;
using ServiceMonitor.Utils;
using ServiceMonitor.ViewModels;
using ServiceMonitor.Views;
using System;
using System.IO;
using System.Windows;

namespace ServiceMonitor
{
    public partial class App : Application
    {
        public static TaskbarIcon? TrayIcon { get; private set; }
        private ServiceRepository _repository = null!;
        private FileStorageService _fileStorage = null!;
        private ServiceCheckerEngine _engine = null!;
        private MainViewModel _mainViewModel = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализация логгера
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<ServiceCheckerEngine>();

            // Создание настроек (можно расширить, добавить из конфига)
            var settings = new AppSettings
            {
                DefaultCheckIntervalMs = 5000
            };

            // Создаём репозиторий и файловый сервис
            _repository = new ServiceRepository();
            var defaultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceState.json");
            _fileStorage = new FileStorageService(defaultFilePath);

            // Создаём движок мониторинга
            _engine = new ServiceCheckerEngine(logger, settings);

            // Загружаем состояние из файла в репозиторий
            var loadedServices = _fileStorage.LoadFromFile();
            if(loadedServices != null)
            {
                foreach(var s in loadedServices)
                    _repository.Add(s);
            }

            // Создаём VM, передаём зависимости
            _mainViewModel = new MainViewModel(_repository, _engine, _fileStorage);

            // Подключаем обработчик к событию обновления статуса сервиса
            _engine.ServiceChecked += (service, result) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var foundService = _repository.Services.FirstOrDefault(s => s == service);
                    if(foundService != null)
                    {
                        foundService.Status = result.Status;
                        foundService.Message = result.Message;
                        foundService.LastStatusChange = result.Timestamp;
                    }

                    Notifier.Show(result.ServiceName, result.Message, result.Status);
                });
            };


            // Инициализируем TaskbarIcon из ресурсов App.xaml
            TrayIcon = (TaskbarIcon)FindResource("TrayIcon");

            // Создаём и показываем главное окно, назначаем DataContext
            MainWindow = new MainWindow
            {
                DataContext = _mainViewModel
            };
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Сохраняем состояние при выходе
            _mainViewModel.SaveState();

            TrayIcon?.Dispose();

            base.OnExit(e);
        }

        // Обработчики меню трея (из App.xaml)
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            if(MainWindow == null)
            {
                MainWindow = new MainWindow
                {
                    DataContext = _mainViewModel
                };
            }

            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if(MainWindow == null)
            {
                MainWindow = new MainWindow
                {
                    DataContext = _mainViewModel
                };
            }

            if(!MainWindow.IsVisible)
                MainWindow.Show();

            if(MainWindow.WindowState == WindowState.Minimized)
                MainWindow.WindowState = WindowState.Normal;

            MainWindow.Activate();
        }
    }
}
