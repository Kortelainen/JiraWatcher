using JiraWatcher.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JiraWatcher
{
    public partial class SettingsWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public SettingsWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.SettingsChanged += ViewModel_SettingsChanged;
        }

        private static readonly Dictionary<string, string> SettingOrder = new Dictionary<string, string>
        {
            { "JiraURL", "Jira URL" },
            { "JiraApiURL", "Jira API URL" },
            { "JiraApiUsername", "Jira API Username" },
            { "JiraApiPassword", "Jira API Password" },
            { "JQL", "Jira ticket seach terms (JQL)" },

        };

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var sortedSettings = SettingOrder.Keys
                .Intersect(Properties.Settings.Default.Properties.Cast<SettingsProperty>().Select(p => p.Name))
                .OrderBy(key => Array.IndexOf(SettingOrder.Keys.ToArray(), key))
                .ToList();

            foreach (var settingName in sortedSettings)
            {
                var setting = Properties.Settings.Default.Properties[settingName] as SettingsProperty;
                if (setting != null && SettingOrder.ContainsKey(setting.Name))
                {
                    int row = SettingsGrid.RowDefinitions.Count;

                    SettingsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    var label = new Label
                    {
                        Content = SettingOrder[setting.Name],
                        Margin = new Thickness(10),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    Grid.SetColumn(label, 0);
                    Grid.SetRow(label, row);
                    SettingsGrid.Children.Add(label);

                    var textBox = new TextBox
                    {
                        Text = Properties.Settings.Default[setting.Name].ToString(),
                        Margin = new Thickness(10),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Tag = setting.Name,
                        Foreground = Brushes.White, 
                    };

                    Grid.SetColumn(textBox, 1);
                    Grid.SetRow(textBox, row);
                    SettingsGrid.Children.Add(textBox);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Children.OfType<TextBox>().ToList().ForEach(textBox =>
                {
                    string settingName = textBox.Tag.ToString();
                    Properties.Settings.Default[settingName] = textBox.Text;
                });
            Properties.Settings.Default.Save();
            ViewModel_SettingsChanged(sender,e);
            MessageBox.Show("Settings saved.");
            Close();
        }


        private void ViewModel_SettingsChanged(object sender, EventArgs e)
        {
            _viewModel.ResetAutoRefreshTimer();
        }
    }
}