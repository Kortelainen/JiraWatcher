using System;
using System.Runtime.InteropServices;

public class FlashWindowHelper
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    [StructLayout(LayoutKind.Sequential)]
    private struct FLASHWINFO
    {
        public uint cbSize;
        public IntPtr hwnd;
        public uint dwFlags;
        public uint uCount;
        public uint dwTimeout;
    }

    private const uint FLASHW_ALL = 3;

    internal static void FlashWindow(IntPtr hwnd, int duration)
    {
        FLASHWINFO fi = new FLASHWINFO();
        fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
        fi.hwnd = hwnd;
        fi.dwFlags = FLASHW_ALL;
        fi.uCount = (uint)(duration / 500);
        fi.dwTimeout = 0;
        FlashWindowEx(ref fi);
    }
}