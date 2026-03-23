using JiraWatcher.Models;
using JiraWatcher.Helpers;
using System;
using System.Linq;
using System.Windows;

namespace JiraWatcher
{
    public partial class SettingsWindow : Window
    {
        private static readonly string[] NotificationSounds = { "Gasp", "Bright Ping", "Exclamation", "Asterisk", "Hand", "Beep", "None" };
        private static readonly string[] ThemeModes = { "Dark", "Light" };

        private readonly MainWindowViewModel _viewModel;

        public SettingsWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NotificationSoundComboBox.ItemsSource = NotificationSounds;
            ThemeModeComboBox.ItemsSource = ThemeModes;

            JiraUrlTextBox.Text = Properties.Settings.Default.JiraURL;
            JiraApiUrlTextBox.Text = Properties.Settings.Default.JiraApiURL;
            JiraApiUsernameTextBox.Text = Properties.Settings.Default.JiraApiUsername;
            JiraApiPasswordTextBox.Text = Properties.Settings.Default.JiraApiPassword;
            NotificationSoundComboBox.SelectedItem = ResolveSelection(Properties.Settings.Default.NotificationSound, NotificationSounds, "Exclamation");
            ThemeModeComboBox.SelectedItem = ResolveSelection(Properties.Settings.Default.ThemeMode, ThemeModes, "Dark");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.JiraURL = JiraUrlTextBox.Text.Trim();
            Properties.Settings.Default.JiraApiURL = JiraApiUrlTextBox.Text.Trim();
            Properties.Settings.Default.JiraApiUsername = JiraApiUsernameTextBox.Text.Trim();
            Properties.Settings.Default.JiraApiPassword = JiraApiPasswordTextBox.Text.Trim();
            Properties.Settings.Default.NotificationSound = NotificationSoundComboBox.SelectedItem as string ?? "Exclamation";
            Properties.Settings.Default.ThemeMode = ThemeModeComboBox.SelectedItem as string ?? "Dark";
            Properties.Settings.Default.Save();

            if (Application.Current is App app)
            {
                app.ApplyTheme(Properties.Settings.Default.ThemeMode);
            }

            _viewModel.ResetAutoRefreshTimer();
            MessageBox.Show("Settings saved.");
            Close();
        }

        private void TestNotificationSoundButton_Click(object sender, RoutedEventArgs e)
        {
            UXEventHelper.PlayNotificationSound(NotificationSoundComboBox.SelectedItem as string ?? "Exclamation");
        }

        private static string ResolveSelection(string value, string[] availableValues, string fallback)
        {
            return availableValues.FirstOrDefault(item => string.Equals(item, value, StringComparison.OrdinalIgnoreCase)) ?? fallback;
        }
    }
}