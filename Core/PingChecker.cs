using ServiceMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMonitor.Core
{
    public class PingChecker : IServiceCheckStrategy
    {
        public async Task<ServiceCheckResult> CheckAsync(ServiceModel model)
        {
            try
            {
                var uri = new Uri(model.Url);
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(uri.Host, 3000);

                return new ServiceCheckResult
                {   
                    ServiceName = model.Name,
                    Status = reply.Status == IPStatus.Success ? ServiceStatus.Ok : ServiceStatus.NotAvailable,
                    Message = $"Ping {uri.Host} - {reply.Status} - {reply.RoundtripTime}ms"
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
