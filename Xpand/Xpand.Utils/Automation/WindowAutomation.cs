using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    public class WindowAutomation {
        public static string FocusedWindowCaption {
            get {
                IntPtr foregroundWindow = Win32Declares.WindowFocus.GetForegroundWindow();
                var stringBuilder = new StringBuilder(Win32Declares.Window.GetWindowTextLength(foregroundWindow) + 1);
                Win32Declares.Window.GetWindowText(foregroundWindow, stringBuilder, stringBuilder.Capacity);
                return stringBuilder.ToString();
            }
        }
        #region FocusWindow
        public static bool FocusWindow(IntPtr windowHandle) {
            if (Win32Declares.WindowFocus.GetForegroundWindow() == windowHandle)
                return true;
            bool ret;
            IntPtr processId1 =
                Win32Declares.Process.GetWindowThreadProcessId(Win32Declares.WindowFocus.GetForegroundWindow(),
                                                               IntPtr.Zero);
            IntPtr processId2 = Win32Declares.Process.GetWindowThreadProcessId(IntPtr.Zero, windowHandle);
            if (processId1 != processId2) {
                Win32Declares.Thread.AttachThreadInput(processId1, processId2, true);
                ret = Win32Declares.WindowFocus.SetForegroundWindow(windowHandle);
                Win32Declares.Thread.AttachThreadInput(processId1, processId2, true);
            } else
                ret = Win32Declares.WindowFocus.SetForegroundWindow(windowHandle);

            Win32Declares.Window.ShowWindowEnum showWindowEnum = Win32Declares.Window.IsIconic(windowHandle)
                                                                     ? Win32Declares.Window.ShowWindowEnum.
                                                                           SW_RESTORE
                                                                     : Win32Declares.Window.ShowWindowEnum.SW_SHOW;
            Win32Declares.Window.ShowWindow(windowHandle, showWindowEnum);
            return ret;
        }

        public static void FocusWindow(string caption) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, caption);
            FocusWindow(findWindow);
        }
        #endregion
        public static Point GetGetWindowPosition(string windowCaption) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
            Win32Types.RECT lpRect;
            Win32Declares.Rect.GetWindowRect(findWindow, out lpRect);
            Rectangle rectangle = lpRect.ToRectangle();
            var position = new Point(rectangle.X, rectangle.Y);
            return position;
        }

        public static bool MoveWindow(string windowCaption, Point newPoisition) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
            Win32Types.RECT lpRect;
            Win32Declares.Rect.GetWindowRect(findWindow, out lpRect);
            Rectangle rectangle = lpRect.ToRectangle();
            bool moveWindow = Win32Declares.Window.MoveWindow(findWindow, newPoisition.X, newPoisition.Y, rectangle.Width,
                                                              rectangle.Height, true);
            Application.DoEvents();

            return moveWindow;
        }

        public static void MinimizeWindow(IntPtr handle) {
            Win32Declares.Window.ShowWindow(handle, Win32Declares.Window.ShowWindowEnum.SW_MINIMIZE);
        }

        public static void MaximizeWindow(IntPtr handle) {
            Win32Declares.Window.ShowWindow(handle, Win32Declares.Window.ShowWindowEnum.SW_SHOWMAXIMIZED);
        }
        #region CloseWindow
        public static bool CloseWindow(IntPtr windowHandle) {
            bool destroyWindow = Win32Declares.Window.DestroyWindow(windowHandle);
            Application.DoEvents();
            return destroyWindow;
        }

        public static bool CloseWindow(string windowCaption) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
            return CloseWindow(findWindow);
        }
        #endregion
    }
}