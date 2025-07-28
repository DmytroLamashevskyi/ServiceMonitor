namespace ServiceMonitor.Models
{
    public static class ServiceTypeValues
    {
        public static List<ServiceType> All => Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>().ToList();
    }
}
