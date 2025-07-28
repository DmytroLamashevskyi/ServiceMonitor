using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceMonitor.Models
{
    public class ServiceModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name = string.Empty;
        private string _url = string.Empty;
        private ServiceType _type = ServiceType.Http;
        private int _updatePeriod = 10000;
        private bool _isActive = true;
        private ServiceStatus _status = ServiceStatus.Warning;
        private DateTime _lastStatusChange = DateTime.Now;
        private string _message = string.Empty;

        public string Name { get => _name; set => SetField(ref _name, value); }
        public string Url { get => _url; set => SetField(ref _url, value); }
        public ServiceType Type { get => _type; set => SetField(ref _type, value); }
        public int UpdatePeriod { get => _updatePeriod; set => SetField(ref _updatePeriod, value); }
        public bool IsActive { get => _isActive; set => SetField(ref _isActive, value); }
        public ServiceStatus Status { get => _status; set => SetField(ref _status, value); }
        public DateTime LastStatusChange { get => _lastStatusChange; set => SetField(ref _lastStatusChange, value); }
        public string Message { get => _message; set => SetField(ref _message, value); }


        private bool _isEditing;
        public bool IsEditing { get => _isEditing; set => SetField(ref _isEditing, value); }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? prop = null)
        {
            if(Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(prop);
            return true;
        }

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(Url) => ValidateUrl(Url),
                    nameof(UpdatePeriod) => ValidateInterval(UpdatePeriod),
                    _ => string.Empty
                };
            }
        }

        public string Error => null!;

        private static string ValidateUrl(string url)
        {
            if(string.IsNullOrWhiteSpace(url))
                return "URL is required.";

            if(!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return "Invalid URL format.";

            if(uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return "Only HTTP/HTTPS are supported.";

            return string.Empty;
        }



        private static string ValidateInterval(int value)
        {
            if(value < 800)
                return "Interval must be at least 100 ms.";
            return string.Empty;
        }
    }
}
