using System.Text.Json;

namespace ServiceMonitor.Models
{
    /// <summary>
    /// Application settings for ServiceCheckerEngine.
    /// Defines logging, concurrency and interval rules.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Path to the log file. Default: ServiceMonitor.log
        /// </summary>
        public string LogFilePath { get; set; } = "ServiceMonitor.log";

        /// <summary>
        /// Maximum log file size in MB before rotation (.old file).
        /// </summary>
        public int MaxLogFileSizeMB { get; set; } = 5;

        /// <summary>
        /// Maximum number of concurrent service checks.
        /// </summary>
        public int MaxParallelChecks { get; set; } = 5;

        /// <summary>
        /// Minimal allowed check interval (ms) to avoid DDoS-like behavior.
        /// </summary>
        public int MinCheckIntervalMs { get; set; } = 700;

        /// <summary>
        /// Default check interval for services if not specified in model (ms).
        /// </summary>
        public int DefaultCheckIntervalMs { get; set; } = 3000;

        /// <summary>
        /// Timeout for HTTP checks in milliseconds.
        /// </summary>
        public int HttpTimeoutMs { get; set; } = 3000;

        /// <summary>
        /// Timeout for Ping checks in milliseconds.
        /// </summary>
        public int PingTimeoutMs { get; set; } = 3000;

        /// <summary>
        /// Returns settings as pretty formatted JSON string.
        /// </summary>
        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
