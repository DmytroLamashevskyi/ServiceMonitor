namespace ServiceMonitor.Models
{
    public enum ServiceType
    {
        Http,
        Ping
    }

    public enum ServiceStatus
    {
        Ok,
        NotAvailable,
        Warning
    }
}
