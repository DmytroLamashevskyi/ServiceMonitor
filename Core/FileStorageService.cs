using ServiceMonitor.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ServiceMonitor.Core
{
    public sealed class FileStorageService
    {
        private readonly string _defaultFilePath;

        public FileStorageService(string defaultFilePath)
        {
            _defaultFilePath = defaultFilePath;
        }

        public ObservableCollection<ServiceModel>? LoadFromFile(string? path = null)
        {
            path ??= _defaultFilePath;
            if(!File.Exists(path))
                return null;

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<ObservableCollection<ServiceModel>>(json);
            }
            catch
            {
                // Логировать или обработать ошибку можно здесь
                return null;
            }
        }

        public bool SaveToFile(ObservableCollection<ServiceModel> services, string? path = null)
        {
            path ??= _defaultFilePath;
            try
            {
                var json = JsonSerializer.Serialize(services, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
                return true;
            }
            catch
            {
                // Логировать или обработать ошибку
                return false;
            }
        }
    }
}
