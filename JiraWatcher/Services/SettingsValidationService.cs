using JiraWatcher.Helpers;

namespace JiraWatcher.Services
{
    internal class SettingsValidationService
    {
        internal static bool AreSettingsValid(string jql)
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.JiraApiURL) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.JiraApiUsername) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.JiraApiPassword) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.JiraURL) &&
                   JqlHelper.IsValidJql(jql);
        }
    }
}
