using JiraWatcher.Helpers;
using JiraWatcher.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JiraWatcher.Models
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<JiraItem> _jiraItems;
        
        public ObservableCollection<JiraItem> JiraItems
        {
            get { return _jiraItems; }
            set
            {
                _jiraItems = value;
                OnPropertyChanged(nameof(JiraItems));
            }
        }

        private DateTime _lastRefreshDateTime;
        public DateTime LastRefreshDateTime
        {
            get { return _lastRefreshDateTime; }
            set
            {
                _lastRefreshDateTime = value;
                OnPropertyChanged(nameof(LastRefreshDateTime));
            }
        }

        private string errorMessage;
        
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }
        public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand OpenLinkCommand { get; }

        public MainWindowViewModel()
        {
            OpenLinkCommand = new RelayCommand<string>(OpenLink);
            JiraItems = new ObservableCollection<JiraItem>();
            LastRefreshDateTime = DateTime.Now;
            StartAutoRefreshTimer();
        }

        private System.Timers.Timer _refreshTimer;
        private static int interval = 60000; // 1min -> Jira max is 500 per 5 min. 15 sec = 25 users. 60 sec = 100 users.

        private void StartAutoRefreshTimer()
        {
            FetchAndRefreshJiraItems();
            _refreshTimer = new System.Timers.Timer();
            _refreshTimer.Interval = interval;
            _refreshTimer.Elapsed += (sender, e) => FetchAndRefreshJiraItems();
            _refreshTimer.Start();
        }

        public event EventHandler SettingsChanged;
        private void OnSettingsChanged() => SettingsChanged?.Invoke(this, EventArgs.Empty);

        internal void ResetAutoRefreshTimer()
        {
            if (_refreshTimer != null)
            { 
                isFirstLoad = true;
                FetchAndRefreshJiraItems();
                _refreshTimer.Stop();
                _refreshTimer.Interval = interval;
                _refreshTimer.Start();
            }
        }

        private bool isFirstLoad = true;

        private async void FetchAndRefreshJiraItems()
        {
            try
            {
                List<JiraItem> fetchedItems = await FetchJiraItemsFromServerAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    JiraItems.Clear();
                    foreach (var item in fetchedItems)
                    {
                        JiraItems.Add(item);
                    }

                    LastRefreshDateTime = DateTime.Now;
                });

                if (!isFirstLoad)
                {
                    foreach (var newItem in fetchedItems)
                    {
                        if (!JiraItems.Any(item => item.Key == newItem.Key))
                        {
                            UXEventHelper.Notification();
                            break;
                        }
                    }
                }
                else
                {
                    isFirstLoad = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Jira items: {ex.Message}");
                ErrorMessage = "An error occurred while fetching Jira items. Please try again later.";
            }
        }

        private async Task<List<JiraItem>> FetchJiraItemsFromServerAsync()
        {
            List<JiraItem> fetchedItems = new List<JiraItem>();

            if (!SettingsValidationService.AreSettingsValid())
            {
                fetchedItems = new List<JiraItem>
                {
                    new JiraItem { Key = "T-1", Summary = "Sample Issue 1", Link = "https://example.com/issue1" },
                    new JiraItem { Key = "T-2", Summary = "Sample Issue 2, This one has longer text. Something that really would go to another line. If list does not fit the window scroll bar is availeable", Link = "https://example.com/issue2" },
                    new JiraItem { Key = "T-3", Summary = "Sample Issue 3, Click the button to open ticket in default browser", Link = "https://example.com/issue3" },
                };
            }
            else
            {
                try
                {
                    JiraService jiraService = new JiraService(new HttpClient());
                    fetchedItems = await jiraService.GetJiraItemsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching Jira items: {ex.Message}");
                    ErrorMessage = "An error occurred while fetching Jira items. Please try again later.";
                }
            }

            return fetchedItems;
        }


        private static void OpenLink(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}