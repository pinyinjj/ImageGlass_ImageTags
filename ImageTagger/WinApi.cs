using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ImageTagger
{
    /// <summary>
    /// Low-level Windows API wrapper for window management and input simulation.
    /// </summary>
    public static class WinApi
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, IntPtr dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const byte VK_LEFT = 0x25;
        public const byte VK_RIGHT = 0x27;

        public static class ImageGlassControl
        {
            private const string ClassName = "WindowsForms10.Window.8.app.0.1ca0_r9_ad1"; 
            private static IntPtr _cachedHwnd = IntPtr.Zero;

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool IsWindow(IntPtr hWnd);

            /// <summary>
            /// Locates the ImageGlass main window handle.
            /// </summary>
            public static IntPtr FindImageGlassWindow()
            {
                if (_cachedHwnd != IntPtr.Zero && IsWindow(_cachedHwnd)) return _cachedHwnd;

                // Try finding by class name
                IntPtr hwnd = FindWindow(ClassName, null);
                if (hwnd != IntPtr.Zero)
                {
                    _cachedHwnd = hwnd;
                    return hwnd;
                }

                // Fallback to title enumeration
                EnumWindows((hWnd, lParam) =>
                {
                    if (!IsWindowVisible(hWnd)) return true;
                    
                    int len = GetWindowTextLength(hWnd);
                    if (len == 0) return true;

                    var sb = new StringBuilder(len + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();

                    if (title.Contains("ImageGlass", StringComparison.OrdinalIgnoreCase))
                    {
                        _cachedHwnd = hWnd;
                        return false;
                    }
                    return true;
                }, IntPtr.Zero);
                
                return _cachedHwnd;
            }

            /// <summary>
            /// Sends a navigation key to ImageGlass.
            /// </summary>
            public static void SendImageGlassKey(Keys key)
            {
                IntPtr hwnd = FindImageGlassWindow();
                if (hwnd == IntPtr.Zero) return;

                SetForegroundWindow(hwnd);

                byte vk = key == Keys.Left ? VK_LEFT : VK_RIGHT;
                keybd_event(vk, 0x45, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                keybd_event(vk, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, IntPtr.Zero);
            }
        }
    }
}