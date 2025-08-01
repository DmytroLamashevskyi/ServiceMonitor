using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using ServiceMonitor.Models;

namespace ServiceMonitor.Core
{
    public sealed class ServiceCheckerEngine : IDisposable
    {
        private readonly ConcurrentDictionary<ServiceModel, CancellationTokenSource> _runningTasks = new();
        private readonly ILogger<ServiceCheckerEngine> _logger;
        private readonly SemaphoreSlim _semaphore;
        private string _logFilePath;
        private readonly int _maxLogFileSizeBytes;
        private readonly int _minCheckIntervalMs;

        private readonly Dictionary<ServiceType, IServiceCheckStrategy> _strategies;
        public event Action<ServiceModel, ServiceCheckResult>? ServiceChecked;

        public ServiceCheckerEngine(ILogger<ServiceCheckerEngine> logger, AppSettings settings)
        {
            _logger = logger;

            // 🔹 Путь к логам
            _logFilePath = settings.LogFilePath ?? "ServiceMonitor.log";

            // Если путь пустой (только имя файла) — сохраняем рядом с exe
            var dir = Path.GetDirectoryName(_logFilePath);
            if(string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                _logFilePath = Path.Combine(dir, _logFilePath);
            }

            Directory.CreateDirectory(dir);

            _maxLogFileSizeBytes = settings.MaxLogFileSizeMB * 1024 * 1024;
            _minCheckIntervalMs = settings.MinCheckIntervalMs;
            _semaphore = new SemaphoreSlim(settings.MaxParallelChecks, settings.MaxParallelChecks);

            // 🔹 Доступные стратегии проверки
            _strategies = new Dictionary<ServiceType, IServiceCheckStrategy>
            {
                [ServiceType.Http] = new HttpChecker(),
                [ServiceType.Ping] = new PingChecker()
            };
        }

        public void StartMonitoring(ServiceModel model)
        {
            StopMonitoring(model);

            if(model.UpdatePeriod < _minCheckIntervalMs)
            {
                _logger?.LogWarning(
                    "Service {Service} requested interval {Interval} ms is too low. Forcing {MinInterval} ms.",
                    model.Name, model.UpdatePeriod, _minCheckIntervalMs);

                model.UpdatePeriod = _minCheckIntervalMs;
            }

            var cts = new CancellationTokenSource();
            _runningTasks[model] = cts;

            _ = Task.Run(() => MonitorServiceAsync(model, cts.Token), cts.Token);
        }

        public void StopMonitoring(ServiceModel model)
        {
            if(_runningTasks.TryRemove(model, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }
        }

        public void StopAll()
        {
            foreach(var kv in _runningTasks)
            {
                kv.Value.Cancel();
                kv.Value.Dispose();
            }
            _runningTasks.Clear();
        }

        private async Task MonitorServiceAsync(ServiceModel model, CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                await _semaphore.WaitAsync(token);
                try
                {
                    var status = await CheckServiceAsync(model);

                    if(status != model.Status)
                        HandleStatusChange(model, status);
                }
                finally
                {
                    _semaphore.Release();
                }

                try
                {
                    await Task.Delay(model.UpdatePeriod, token);
                }
                catch(TaskCanceledException) { }
            }
        }

        private async Task<ServiceStatus> CheckServiceAsync(ServiceModel model)
        {
            try
            {
                if(!_strategies.TryGetValue(model.Type, out var strategy))
                    return ServiceStatus.Error;

                var result = await strategy.CheckAsync(model);
                return result.Status;
            }
            catch(Exception ex)
            {
                _logger?.LogCritical(ex, "Critical error during check for {Service}", model.Name);
                return ServiceStatus.Error;
            }
        }

        private void HandleStatusChange(ServiceModel model, ServiceStatus newStatus)
        {
            model.Status = newStatus;
            model.LastStatusChange = DateTime.Now;

            var message = $"Service ({model.Url}) changed status to {newStatus}";

            WriteLog(message, newStatus is ServiceStatus.Error or ServiceStatus.Offline);

            ServiceChecked?.Invoke(model, new ServiceCheckResult
            {
                ServiceName = model.Name,
                Status = newStatus,
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        private void WriteLog(string message, bool error)
        {
            try
            {
                RotateLogIfNeeded();
                File.AppendAllText(_logFilePath, message + Environment.NewLine, Encoding.UTF8);

                if(error)
                    _logger?.LogError(message);
                else
                    _logger?.LogInformation(message);
            }
            catch
            {
                // ignore logging errors
            }
        }

        private void RotateLogIfNeeded()
        {
            if(!File.Exists(_logFilePath))
                return;

            var fi = new FileInfo(_logFilePath);
            if(fi.Length > _maxLogFileSizeBytes)
            {
                var oldPath = _logFilePath + ".old";
                if(File.Exists(oldPath))
                    File.Delete(oldPath);
                File.Move(_logFilePath, oldPath);
            }
        }

        public void Dispose()
        {
            StopAll();
            _semaphore.Dispose();
        }
    }
}
