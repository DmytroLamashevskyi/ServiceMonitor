using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceMonitor.Models
{
    /// <summary>
    /// Represents a single monitored service with validation support for WPF.
    /// </summary>
    public class ServiceModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name = string.Empty;
        private string _url = string.Empty;
        private ServiceType _type = ServiceType.Http;
        private int _updatePeriod = 10000;
        private bool _isActive = true;
        private ServiceStatus _status = ServiceStatus.Unknown;
        private DateTime _lastStatusChange = DateTime.Now;
        private string _message = string.Empty;
        private bool _isEditing;

        /// <summary>Display name of the service.</summary>
        public string Name { get => _name; set => SetField(ref _name, value); }

        /// <summary>Target URL of the service (HTTP or Ping host).</summary>
        public string Url { get => _url; set => SetField(ref _url, value); }

        /// <summary>Check type: HTTP or Ping.</summary>
        public ServiceType Type { get => _type; set => SetField(ref _type, value); }

        /// <summary>Check interval in milliseconds.</summary>
        public int UpdatePeriod { get => _updatePeriod; set => SetField(ref _updatePeriod, value); }

        /// <summary>Indicates if the service is active and should be monitored.</summary>
        public bool IsActive { get => _isActive; set => SetField(ref _isActive, value); }

        /// <summary>Current status of the service.</summary>
        public ServiceStatus Status { get => _status; set => SetField(ref _status, value); }

        /// <summary>Date and time of the last status change.</summary>
        public DateTime LastStatusChange { get => _lastStatusChange; set => SetField(ref _lastStatusChange, value); }

        /// <summary>Additional info or last error message.</summary>
        public string Message { get => _message; set => SetField(ref _message, value); }

        /// <summary>Indicates if the row is in edit mode in the UI.</summary>
        public bool IsEditing { get => _isEditing; set => SetField(ref _isEditing, value); }

        /// <inheritdoc/>
        public string Error => string.Empty;

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

        /// <inheritdoc/>
        public string this[string columnName] => columnName switch
        {
            nameof(Url) => ValidateUrl(Url),
            nameof(UpdatePeriod) => ValidateInterval(UpdatePeriod),
            _ => string.Empty
        };

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
            if(value < 700)
                return "Interval must be at least 700 ms.";
            return string.Empty;
        }
    }
}
