using Microsoft.Win32;
using ServiceMonitor.Core;
using ServiceMonitor.Models;
using ServiceMonitor.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ServiceMonitor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string _stateFileName = "ServiceState.json";

        private readonly ServiceRepository _repository;
        private readonly ServiceCheckerEngine _engine;
        private readonly FileStorageService _fileStorage;

        public ObservableCollection<ServiceModel> Services => _repository.Services;

        private ICommand? _editCommand;
        private ICommand? _saveCommand;
        private ICommand? _deleteCommand;

        public ICommand EditCommand => _editCommand ??= new RelayCommand<ServiceModel>(service =>
        {
            if(service != null)
                service.IsEditing = true;
        });

        public ICommand SaveCommand => _saveCommand ??= new RelayCommand<ServiceModel>(service =>
        {
            if(service == null) return;

            service.IsEditing = false;

            if(service.IsActive)
            {
                _engine.StopMonitoring(service);
                _engine.StartMonitoring(service);
            }
            else
            {
                _engine.StopMonitoring(service);
            }
        });

        public ICommand DeleteCommand => _deleteCommand ??= new RelayCommand<ServiceModel>(service =>
        {
            if(service == null) return;

            _engine.StopMonitoring(service);
            _repository.Remove(service);
        });

        public MainViewModel(ServiceRepository repository, ServiceCheckerEngine engine, FileStorageService fileStorage)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));

            LoadState();

            foreach(var service in Services)
            {
                if(service.IsActive)
                    _engine.StartMonitoring(service);
            }
        }


        public void AddNewService()
        {
            var service = new ServiceModel
            {
                Name = "New Service",
                Url = "http://localhost",
                Type = ServiceType.Http,
                UpdatePeriod = 5000,
                IsActive = true
            };

            _repository.Add(service);
            _engine.StartMonitoring(service);
        }

        public void RemoveService(ServiceModel service)
        {
            if(service == null) return;

            _engine.StopMonitoring(service);
            _repository.Remove(service);
        }

        #region Saving & Loading JSON

        public void SaveState()
        {
            try
            {
                _fileStorage.SaveToFile(Services, _stateFileName);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error saving state: {ex.Message}");
            }
        }

        public void LoadState()
        {
            try
            {
                var list = _fileStorage.LoadFromFile(_stateFileName);
                if(list != null)
                {
                    Services.Clear();
                    foreach(var s in list)
                        Services.Add(s);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error loading state: {ex.Message}");
            }
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                _fileStorage.SaveToFile(Services, filePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                var list = _fileStorage.LoadFromFile(filePath);
                if(list != null)
                {
                    foreach(var s in Services)
                        _engine.StopMonitoring(s);

                    Services.Clear();

                    foreach(var s in list)
                    {
                        Services.Add(s);
                        if(s.IsActive)
                            _engine.StartMonitoring(s);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading from file: {ex.Message}");
            }
        }

        #endregion

        public void ExportServices()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = "ServiceList.json"
            };

            if(dialog.ShowDialog() == true)
            {
                try
                {
                    SaveToFile(dialog.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error saving to file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void ImportServices()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            if(dialog.ShowDialog() == true && System.IO.File.Exists(dialog.FileName))
            {
                try
                {
                    LoadFromFile(dialog.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error loading from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
