using System.Diagnostics;

namespace JiraWatcher.Helpers
{
    internal static class UXEventHelper
    {
        internal static void Notification()
        {
            System.Media.SystemSounds.Beep.Play();
            FlashWindowHelper.FlashWindow(Process.GetCurrentProcess().MainWindowHandle, 2000);
        }

    }
}