using ServiceMonitor.Models;
using System.Collections.Concurrent;

namespace ServiceMonitor.Core;

public class ServiceCheckerEngine
{
    private readonly ConcurrentDictionary<ServiceModel, CancellationTokenSource> _runningTasks = new();

    public void StartMonitoring(ServiceModel model, Action<ServiceModel, ServiceCheckResult> onChecked)
    {
        StopMonitoring(model); // перезапуск если уже был запущен

        var cts = new CancellationTokenSource();
        _runningTasks[model] = cts;

        Task.Run(async () =>
        {
            IServiceCheckStrategy strategy = model.Type switch
            {
                ServiceType.Http => new HttpChecker(),
                ServiceType.Ping => new PingChecker(),
                _ => throw new NotImplementedException()
            };

            while(!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await strategy.CheckAsync(model);

                    // вызываем в VM
                    onChecked(model, result);
                }
                catch(Exception ex)
                {
                    onChecked(model, new ServiceCheckResult
                    {
                        CheckedAt = DateTime.Now,
                        Status = ServiceStatus.Warning,
                        Message = ex.Message,
                       
                    });
                }

                await Task.Delay(model.UpdatePeriod, cts.Token);
            }
        }, cts.Token);
    }

    public void StopMonitoring(ServiceModel model)
    {
        if(_runningTasks.TryRemove(model, out var cts))
            cts.Cancel();
    }

    public void StopAll()
    {
        foreach(var cts in _runningTasks.Values)
            cts.Cancel();

        _runningTasks.Clear();
    }
}
