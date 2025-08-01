namespace ServiceMonitor.Models
{
    /// <summary>
    /// Unified result of a service check.
    /// </summary>
    public sealed class ServiceCheckResult
    {
        /// <summary>
        /// Display name of the service.
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Current detected status.
        /// </summary>
        public ServiceStatus Status { get; set; } = ServiceStatus.Unknown;

        /// <summary>
        /// Optional descriptive message (e.g., "HTTP 200 OK" or "Ping Timeout").
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Time of the check result creation.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Returns a readable string representation of the result for logs or UI.
        /// </summary>
        public override string ToString()
        {
            return $"[{Timestamp:u}] {ServiceName} => {Status} ({Message})";
        }
    }
}
