using JiraWatcher.Helpers;
using JiraWatcher.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JiraWatcher.Models
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string DefaultJql = "created >= -2d order by created DESC";
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(1);

        private string _errorMessage = string.Empty;
        private JiraTab? _selectedTab;
        private System.Timers.Timer? _refreshTimer;
        private bool _isFirstLoad = true;
        private bool _isRefreshing;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<JiraTab> Tabs { get; } = new ObservableCollection<JiraTab>();

        public JiraTab? SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab == value)
                {
                    return;
                }

                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand OpenLinkCommand { get; }

        public ICommand AddTabCommand { get; }

        public MainWindowViewModel()
        {
            OpenLinkCommand = new RelayCommand<string>(OpenLink);
            AddTabCommand = new RelayCommand<object>(_ => AddTab());
            LoadTabs();
            StartAutoRefreshTimer();
        }

        private void StartAutoRefreshTimer()
        {
            _ = RefreshAllTabsAsync();
            _refreshTimer = new System.Timers.Timer(RefreshInterval.TotalMilliseconds);
            _refreshTimer.Elapsed += (sender, e) => _ = RefreshAllTabsAsync();
            _refreshTimer.Start();
        }

        internal void ResetAutoRefreshTimer()
        {
            if (_refreshTimer == null)
            {
                return;
            }

            SaveTabs();
            _isFirstLoad = true;
            _ = RefreshAllTabsAsync();
            _refreshTimer.Stop();
            _refreshTimer.Interval = RefreshInterval.TotalMilliseconds;
            _refreshTimer.Start();
        }

        internal void UpdateTabSettings(JiraTab tab, string tabName, string jql)
        {
            tab.Name = string.IsNullOrWhiteSpace(tabName) ? GenerateNextTabName() : tabName.Trim();
            tab.Jql = string.IsNullOrWhiteSpace(jql) ? GetDefaultJql() : jql.Trim();
            SaveTabs();
        }

        internal void DeleteTab(JiraTab tab)
        {
            if (Tabs.Count <= 1)
            {
                return;
            }

            int index = Tabs.IndexOf(tab);
            Tabs.Remove(tab);

            if (SelectedTab == tab || SelectedTab == null)
            {
                SelectedTab = Tabs[Math.Max(0, Math.Min(index, Tabs.Count - 1))];
            }

            SaveTabs();
        }

        private void AddTab()
        {
            JiraTab tab = new JiraTab
            {
                Name = GenerateNextTabName(),
                Jql = GetDefaultJql()
            };

            Tabs.Add(tab);
            SelectedTab = tab;
            SaveTabs();
            _ = RefreshAllTabsAsync();
        }

        private void LoadTabs()
        {
            foreach (JiraTab tab in LoadPersistedTabs())
            {
                Tabs.Add(tab);
            }

            if (Tabs.Count == 0)
            {
                Tabs.Add(new JiraTab
                {
                    Name = "Search 1",
                    Jql = GetDefaultJql()
                });
            }

            SelectedTab = Tabs.FirstOrDefault();
            SaveTabs();
        }

        private IEnumerable<JiraTab> LoadPersistedTabs()
        {
            string persistedTabsJson = Properties.Settings.Default.JiraTabsJson;

            if (string.IsNullOrWhiteSpace(persistedTabsJson))
            {
                yield return new JiraTab
                {
                    Name = "Search 1",
                    Jql = GetDefaultJql()
                };

                yield break;
            }

            List<PersistedJiraTab>? persistedTabs;

            try
            {
                persistedTabs = JsonSerializer.Deserialize<List<PersistedJiraTab>>(persistedTabsJson);
            }
            catch (JsonException)
            {
                persistedTabs = null;
            }

            if (persistedTabs == null || persistedTabs.Count == 0)
            {
                yield return new JiraTab
                {
                    Name = "Search 1",
                    Jql = GetDefaultJql()
                };

                yield break;
            }

            foreach (PersistedJiraTab persistedTab in persistedTabs)
            {
                yield return new JiraTab
                {
                    Id = persistedTab.Id == Guid.Empty ? Guid.NewGuid() : persistedTab.Id,
                    Name = string.IsNullOrWhiteSpace(persistedTab.Name) ? GenerateNextTabName() : persistedTab.Name.Trim(),
                    Jql = string.IsNullOrWhiteSpace(persistedTab.Jql) ? GetDefaultJql() : persistedTab.Jql.Trim()
                };
            }
        }

        private async Task RefreshAllTabsAsync()
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            try
            {
                ErrorMessage = string.Empty;
                List<JiraTab> tabs = Application.Current.Dispatcher.Invoke(() => Tabs.ToList());

                foreach (JiraTab tab in tabs)
                {
                    await RefreshTabAsync(tab, !_isFirstLoad);
                }

                _isFirstLoad = false;
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private async Task RefreshTabAsync(JiraTab tab, bool notifyOnNewItems)
        {
            HashSet<string> existingKeys = Application.Current.Dispatcher.Invoke(() =>
                tab.JiraItems
                    .Where(item => !string.IsNullOrWhiteSpace(item.Key))
                    .Select(item => item.Key!)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase));

            List<JiraItem> fetchedItems = await FetchJiraItemsFromServerAsync(tab.Jql);

            Application.Current.Dispatcher.Invoke(() =>
            {
                tab.JiraItems.Clear();

                foreach (JiraItem item in fetchedItems)
                {
                    tab.JiraItems.Add(item);
                }

                tab.LastRefreshDateTime = DateTime.Now;
            });

            if (notifyOnNewItems && fetchedItems.Any(item => !string.IsNullOrWhiteSpace(item.Key) && !existingKeys.Contains(item.Key!)))
            {
                UXEventHelper.Notification();
            }
        }

        private async Task<List<JiraItem>> FetchJiraItemsFromServerAsync(string jql)
        {
            if (!SettingsValidationService.AreSettingsValid(jql))
            {
                return new List<JiraItem>
                {
                    new JiraItem { Key = "T-1", Summary = "Sample Issue 1", Link = "https://example.com/issue1" },
                    new JiraItem { Key = "T-2", Summary = "Sample Issue 2, This one has longer text. Something that really would go to another line. If list does not fit the window scroll bar is availeable", Link = "https://example.com/issue2" },
                    new JiraItem { Key = "T-3", Summary = "Sample Issue 3, Click the button to open ticket in default browser", Link = "https://example.com/issue3" },
                };
            }

            try
            {
                using JiraService jiraService = new JiraService(new HttpClient());
                return await jiraService.GetJiraItemsAsync(jql);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Jira items: {ex.Message}");
                ErrorMessage = "An error occurred while fetching Jira items. Please try again later.";
                return new List<JiraItem>();
            }
        }

        private void SaveTabs()
        {
            List<PersistedJiraTab> persistedTabs = Tabs
                .Select(tab => new PersistedJiraTab
                {
                    Id = tab.Id,
                    Name = tab.Name,
                    Jql = tab.Jql
                })
                .ToList();

            Properties.Settings.Default.JiraTabsJson = JsonSerializer.Serialize(persistedTabs);
            Properties.Settings.Default.Save();
        }

        private string GenerateNextTabName()
        {
            int tabNumber = 1;

            while (Tabs.Any(tab => string.Equals(tab.Name, $"Search {tabNumber}", StringComparison.OrdinalIgnoreCase)))
            {
                tabNumber++;
            }

            return $"Search {tabNumber}";
        }

        private static string GetDefaultJql()
        {
            return string.IsNullOrWhiteSpace(Properties.Settings.Default.JQL)
                ? DefaultJql
                : Properties.Settings.Default.JQL.Trim();
        }

        private static void OpenLink(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

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

        private sealed class PersistedJiraTab
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Jql { get; set; } = string.Empty;
        }
    }
}