using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace JiraWatcher.Models
{
    public class JiraTab : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _jql = string.Empty;
        private DateTime _lastRefreshDateTime = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Jql
        {
            get => _jql;
            set
            {
                if (_jql == value)
                {
                    return;
                }

                _jql = value;
                OnPropertyChanged(nameof(Jql));
            }
        }

        public ObservableCollection<JiraItem> JiraItems { get; } = new ObservableCollection<JiraItem>();

        public DateTime LastRefreshDateTime
        {
            get => _lastRefreshDateTime;
            set
            {
                if (_lastRefreshDateTime == value)
                {
                    return;
                }

                _lastRefreshDateTime = value;
                OnPropertyChanged(nameof(LastRefreshDateTime));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}