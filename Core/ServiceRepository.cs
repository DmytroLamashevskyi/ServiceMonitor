using ServiceMonitor.Models;
using System.Collections.ObjectModel;

namespace ServiceMonitor.Core
{
    public class ServiceRepository
    {
        public ObservableCollection<ServiceModel> Services { get; } = new ObservableCollection<ServiceModel>();

        public void Add(ServiceModel service)
        {
            if(service != null && !Services.Contains(service))
                Services.Add(service);
        }

        public void Remove(ServiceModel service)
        {
            if(service != null && Services.Contains(service))
                Services.Remove(service);
        }
    }
}
