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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            MainWindowViewModel viewModel = new MainWindowViewModel();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
