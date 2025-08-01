using ServiceMonitor.Models;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ServiceMonitor.Core
{
    /// <summary>
    /// Service check strategy using ICMP Ping.
    /// </summary>
    public sealed class PingChecker : IServiceCheckStrategy
    {
        /// <summary>
        /// Checks service availability via ICMP Ping.
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
                        Message = "Empty URL for Ping check"
                    };
                }

                string host;
                try
                {
                    host = new Uri(model.Url).Host;
                }
                catch
                {
                    // Fallback if user passed just hostname or IP without scheme
                    host = model.Url;
                }

                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, 3000);

                var status = reply.Status == IPStatus.Success
                    ? ServiceStatus.Online
                    : ServiceStatus.Offline;

                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = status,
                    Message = $"Ping {host} → {reply.Status} ({reply.RoundtripTime} ms)"
                };
            }
            catch(Exception ex)
            {
                return new ServiceCheckResult
                {
                    ServiceName = model.Name,
                    Status = ServiceStatus.Error,
                    Message = $"Ping failed: {ex.Message}"
                };
            }
        }
    }
}
