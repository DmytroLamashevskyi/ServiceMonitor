using ServiceMonitor.Models;

namespace ServiceMonitor.Core
{
    /// <summary>
    /// Strategy interface for performing service health checks.
    /// Implementations should define how a specific service type is checked (HTTP, Ping, etc.).
    /// </summary>
    public interface IServiceCheckStrategy
    {
        /// <summary>
        /// Executes a health check for the given service model asynchronously.
        /// </summary>
        /// <param name="model">The service model with configuration for the check.</param>
        /// <returns>
        /// A <see cref="ServiceCheckResult"/> with the status and any descriptive message.
        /// </returns>
        Task<ServiceCheckResult> CheckAsync(ServiceModel model);
    }
}
