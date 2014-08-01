using System;
using System.Runtime.InteropServices;

namespace Xpand.EasyTest {
    public class Win32 {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public static void CloseWindow() {
            CloseWindow(GetForegroundWindow());
        }

        public static void CloseWindow(IntPtr window){
            SendMessage(window, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        public static void CloseWindow(string windowName){
            var window = FindWindowByCaption(IntPtr.Zero, windowName);
            CloseWindow(window);
        }

    }
}
