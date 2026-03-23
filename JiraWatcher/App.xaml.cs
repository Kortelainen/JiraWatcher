using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using JiraWatcher.Models;

namespace JiraWatcher
{
    public partial class App : Application
    {
        private static readonly Uri DarkThemeUri = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml", UriKind.Absolute);
        private static readonly Uri LightThemeUri = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml", UriKind.Absolute);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplyTheme(JiraWatcher.Properties.Settings.Default.ThemeMode);
            MainWindow mainWindow = new MainWindow();
            MainWindowViewModel viewModel = new MainWindowViewModel();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }

        internal void ApplyTheme(string? themeMode)
        {
            Uri targetThemeUri = string.Equals(themeMode, "Light", StringComparison.OrdinalIgnoreCase)
                ? LightThemeUri
                : DarkThemeUri;

            int themeDictionaryIndex = Resources.MergedDictionaries
                .Select((dictionary, index) => new { dictionary, index })
                .FirstOrDefault(item =>
                    item.dictionary.Source != null &&
                    (item.dictionary.Source.OriginalString.Contains("MaterialDesignTheme.Dark.xaml", StringComparison.OrdinalIgnoreCase) ||
                     item.dictionary.Source.OriginalString.Contains("MaterialDesignTheme.Light.xaml", StringComparison.OrdinalIgnoreCase)))?
                .index ?? -1;

            ResourceDictionary themeDictionary = new ResourceDictionary
            {
                Source = targetThemeUri
            };

            if (themeDictionaryIndex >= 0)
            {
                Resources.MergedDictionaries[themeDictionaryIndex] = themeDictionary;
            }
            else
            {
                Resources.MergedDictionaries.Insert(0, themeDictionary);
            }
        }
    }
}
