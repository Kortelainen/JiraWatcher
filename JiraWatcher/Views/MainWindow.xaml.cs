using JiraWatcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JiraWatcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }

            SettingsWindow settingsWindow = new SettingsWindow(viewModel);
            settingsWindow.ShowDialog();
        }

        private void TabSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element
                && element.DataContext is JiraTab tab
                && DataContext is MainWindowViewModel viewModel)
            {
                TabSettingsWindow tabSettingsWindow = new TabSettingsWindow(viewModel, tab);
                tabSettingsWindow.Owner = this;
                tabSettingsWindow.ShowDialog();
            }
        }
    }
}
