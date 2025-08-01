using ServiceMonitor.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceMonitor.Core
{
    /// <summary>
    /// Service check strategy using HTTP GET request.
    /// </summary>
    public sealed class HttpChecker : IServiceCheckStrategy
    {
        private static readonly HttpClient _client = new()
        {
            Timeout = TimeSpan.FromSeconds(5) // Limit to avoid long hangs
        };

        /// <summary>
        /// Performs an HTTP GET to check service availability.
        /// </summary>
        public async Task<ServiceCheckResult> CheckAsync(ServiceModel model)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(model.Url))
                {
                    return new ServiceCheckResult
                    {
                        ServiceName = model.Name,
                        Status = ServiceStatus.Error,
                        Message = "Empty URL for HTTP check"
                    };
                }

                using var response = await _client.GetAsync(model.Url);

                var status = response.IsSuccessStatusCode
                    ? ServiceStatus.Online
                    : ServiceStatus.Offline;

                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = status,
                    Message = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }
            catch(TaskCanceledException)
            {
                // Timeout
                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = ServiceStatus.Offline,
                    Message = "HTTP request timed out"
                };
            }
            catch(HttpRequestException ex)
            {
                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = ServiceStatus.Offline,
                    Message = $"HTTP request failed: {ex.Message}"
                };
            }
            catch(Exception ex)
            {
                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = ServiceStatus.Error,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
