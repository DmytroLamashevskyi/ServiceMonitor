namespace ServiceMonitor.Models
{
    /// <summary>
    /// Provides cached collections of available ServiceType enum values.
    /// </summary>
    public static class ServiceTypeValues
    {
        /// <summary>
        /// Cached list of all ServiceType enum values.
        /// </summary>
        public static IReadOnlyList<ServiceType> All { get; } =
            Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>().ToList();

        /// <summary>
        /// Cached dictionary of ServiceType enum values to their names.
        /// </summary>
        public static IReadOnlyDictionary<ServiceType, string> Names { get; } =
            All.ToDictionary(v => v, v => v.ToString());
    }
}
