using ServiceMonitor.Core;
using ServiceMonitor.Models;
using ServiceMonitor.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace ServiceMonitor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ServiceCheckerEngine _engine = new();

        private const string _configPath = "config.json";
        public ObservableCollection<ServiceModel> Services { get; set; } = new();
        public ICommand EditCommand => new RelayCommand<ServiceModel>(s => s.IsEditing = true);
        public ICommand SaveCommand => new RelayCommand<ServiceModel>(s => {

            s.IsEditing = false;
            _engine.StartMonitoring(s, OnServiceChecked);
        });

        public ICommand DeleteCommand => new RelayCommand<ServiceModel>(s => {
            _engine.StopMonitoring(s); 
            Services.Remove(s);
        });

        public ICommand TestCommand => new RelayCommand<ServiceModel>(model =>
        {
            _ = MessageBox.Show($"Testing {model.Name} at {model.Url}", "Test Service");
        });

        public MainViewModel()
        {
            foreach(var service in Services)
            {
                if(service.IsActive)
                {
                    _engine.StartMonitoring(service, OnServiceChecked);
                }
            }
        }

        private void OnServiceChecked(ServiceModel model, ServiceCheckResult result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if(model.Status != result.Status)
                {
                    model.LastStatusChange = result.CheckedAt;
                    Notifier.Show(result.ServiceName ,result.Message, result.Status);
                }

                model.Status = result.Status;
                model.Message = result.Message;
            });
        }

        public void AddNewService()
        {
            Services.Add(new ServiceModel
            {
                Name = "New Service",
                Url = "https://example.com/health",
                Type = ServiceType.Http,
                UpdatePeriod = 10000,
                IsActive = true,
                IsEditing = true,
                Status = ServiceStatus.Ok,
                LastStatusChange = DateTime.Now,
                Message = "Новый сервис добавлен"
            });
        }

        public void SaveToFile(string path = _configPath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(Services, options);
                File.WriteAllText(path, json);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        public void LoadFromFile(string path = _configPath)
        {
            try
            {
                if(!File.Exists(path)) return;

                var json = File.ReadAllText(path);
                var loadedServices = JsonSerializer.Deserialize<ObservableCollection<ServiceModel>>(json);

                if(loadedServices != null)
                {
                    Services.Clear();
                    foreach(var s in loadedServices)
                        Services.Add(s);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }
    }
}
