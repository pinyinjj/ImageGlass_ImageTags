using System;
using System.Runtime.InteropServices;
using System.Text; // For StringBuilder
using System.Windows.Forms; // For Keys enum
using System.Linq;

namespace ImageTagger
{
    public static class WinApi
    {
        // FindWindow API (by ClassName and WindowName)
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // EnumWindows API (to enumerate all top-level windows)
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        // GetWindowText API (to get window title)
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // GetWindowTextLength API (to get window title length)
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        // Delegate for EnumWindows callback
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // SetForegroundWindow API
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // SetWindowPos API for controlling Z-order (TopMost)
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // HWND values for SetWindowPos
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        // SWP flags for SetWindowPos
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_NOACTIVATE = 0x0010;


        // keybd_event API
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, IntPtr dwExtraInfo);

        // Key event constants
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int VK_LEFT = 0x25;
        public const int VK_RIGHT = 0x27;

        public static class ImageGlassControl
        {
            // Class name for ImageGlass 9 (may vary slightly for different versions or if custom themed)
            private const string ImageGlassWindowClassName = "WindowsForms10.Window.8.app.0.1ca0_r9_ad1"; 
            private const string ImageGlassPartialWindowName = "ImageGlass"; // Common part of the window title

            // Store the found ImageGlass window handle for efficiency
            private static IntPtr _imageGlassHwnd = IntPtr.Zero;

            /// <summary>
            /// Attempts to find the ImageGlass window by class name or by partial title.
            /// </summary>
            /// <returns>The handle of the ImageGlass window, or IntPtr.Zero if not found.</returns>
            private static IntPtr FindImageGlassWindow()
            {
                if (_imageGlassHwnd != IntPtr.Zero && IsWindow(_imageGlassHwnd))
                {
                    return _imageGlassHwnd; // Use cached handle if still valid
                }

                // Try to find by class name first (most reliable for WinForms apps)
                IntPtr hwnd = FindWindow(ImageGlassWindowClassName, null);
                if (hwnd != IntPtr.Zero)
                {
                    _imageGlassHwnd = hwnd;
                    return hwnd;
                }

                // Fallback: enumerate windows and search by partial title
                IntPtr foundHwnd = IntPtr.Zero;
                EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
                {
                    int length = GetWindowTextLength(hWnd);
                    if (length == 0) return true; // Continue enumeration if no title

                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    string windowTitle = sb.ToString();

                    if (windowTitle.Contains(ImageGlassPartialWindowName, StringComparison.OrdinalIgnoreCase))
                    {
                        foundHwnd = hWnd;
                        return false; // Stop enumeration
                    }
                    return true; // Continue enumeration
                }, IntPtr.Zero);
                
                _imageGlassHwnd = foundHwnd; // Cache the found handle
                return foundHwnd;
            }

            // IsWindow API
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool IsWindow(IntPtr hWnd);

            public static void SendImageGlassKey(Keys key)
            {
                IntPtr imageGlassHwnd = FindImageGlassWindow();
                
                if (imageGlassHwnd != IntPtr.Zero)
                {
                    // Activate ImageGlass window
                    SetForegroundWindow(imageGlassHwnd);

                    byte vkCode;
                    switch (key)
                    {
                        case Keys.Left:
                            vkCode = VK_LEFT;
                            break;
                        case Keys.Right:
                            vkCode = VK_RIGHT;
                            break;
                        default:
                            return; // Only support Left/Right for now
                    }

                    // Press the key
                    keybd_event(vkCode, 0x45, KEYEVENTF_EXTENDEDKEY | 0, IntPtr.Zero);
                    // Release the key
                    keybd_event(vkCode, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, IntPtr.Zero);
                }
                // No MessageBox.Show here, let MainForm handle logging.
            }
        }
    }
}
