using JiraWatcher.Models;
using System;
using System.Windows;

namespace JiraWatcher
{
    public partial class SettingsWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public SettingsWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            JiraUrlTextBox.Text = Properties.Settings.Default.JiraURL;
            JiraApiUrlTextBox.Text = Properties.Settings.Default.JiraApiURL;
            JiraApiUsernameTextBox.Text = Properties.Settings.Default.JiraApiUsername;
            JiraApiPasswordTextBox.Text = Properties.Settings.Default.JiraApiPassword;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.JiraURL = JiraUrlTextBox.Text.Trim();
            Properties.Settings.Default.JiraApiURL = JiraApiUrlTextBox.Text.Trim();
            Properties.Settings.Default.JiraApiUsername = JiraApiUsernameTextBox.Text.Trim();
            Properties.Settings.Default.JiraApiPassword = JiraApiPasswordTextBox.Text.Trim();
            Properties.Settings.Default.Save();
            _viewModel.ResetAutoRefreshTimer();
            MessageBox.Show("Settings saved.");
            Close();
        }
    }
}