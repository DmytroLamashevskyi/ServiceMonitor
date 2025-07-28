namespace ServiceMonitor.Models
{
    public class ServiceCheckResult
    {
        public ServiceStatus Status { get; set; }
        public string ServiceName { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CheckedAt { get; set; } = DateTime.Now;
    }
}
