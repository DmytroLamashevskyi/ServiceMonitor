using ServiceMonitor.Models;

namespace ServiceMonitor.Core
{
    public interface IServiceCheckStrategy
    {
        Task<ServiceCheckResult> CheckAsync(ServiceModel model);
    }
}
