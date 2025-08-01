namespace ServiceMonitor.Models
{
    /// <summary>
    /// Defines the method used to check the service availability.
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// Service will be checked via HTTP/HTTPS request.
        /// </summary>
        Http = 0,

        /// <summary>
        /// Service will be checked using ICMP ping.
        /// </summary>
        Ping = 1
    }

    /// <summary>
    /// Represents the current availability status of a monitored service.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// Service is fully operational and responding as expected.
        /// </summary>
        Online = 0,

        /// <summary>
        /// Service did not respond or is unreachable.
        /// </summary>
        Offline = 1,

        /// <summary>
        /// Service responded with delay or partial issues (e.g., timeout or unstable connection).
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Service check failed due to an internal monitoring error or unexpected exception.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Service status could not be determined (unexpected state or uninitialized).
        /// </summary>
        Unknown = 4
    }
}
