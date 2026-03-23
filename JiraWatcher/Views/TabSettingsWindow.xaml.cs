using JiraWatcher.Models;
using System.Windows;

namespace JiraWatcher
{
    public partial class TabSettingsWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly JiraTab _tab;

        public TabSettingsWindow(MainWindowViewModel viewModel, JiraTab tab)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _tab = tab;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TabNameTextBox.Text = _tab.Name;
            TabJqlTextBox.Text = _tab.Jql;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateTabSettings(_tab, TabNameTextBox.Text, TabJqlTextBox.Text);
            _viewModel.ResetAutoRefreshTimer();
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Tabs.Count <= 1)
            {
                MessageBox.Show("Cannot delete the last tab.", "Delete Tab", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Delete tab \"{_tab.Name}\"?",
                "Delete Tab",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.DeleteTab(_tab);
                Close();
            }
        }
    }
}
