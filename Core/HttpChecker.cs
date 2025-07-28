using ServiceMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMonitor.Core
{
    public class HttpChecker : IServiceCheckStrategy
    {
        private static readonly HttpClient _client = new();

        public async Task<ServiceCheckResult> CheckAsync(ServiceModel model)
        {
            try
            {
                var response = await _client.GetAsync(model.Url);
                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = response.IsSuccessStatusCode ? ServiceStatus.Ok : ServiceStatus.NotAvailable,
                    Message = $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}"
                };
            }
            catch(Exception ex)
            {
                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = ServiceStatus.Warning,
                    Message = ex.Message
                };
            }
        }
    }
}
