using JiraWatcher.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraWatcher.Services
{
    internal class SettingsValidationService
    {
        internal static bool AreSettingsValid()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.JiraApiURL) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.JiraApiUsername) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.JiraApiPassword) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.JiraURL) &&
                   JqlHelper.IsValidJql(Properties.Settings.Default.JQL);
        }
    }
}
