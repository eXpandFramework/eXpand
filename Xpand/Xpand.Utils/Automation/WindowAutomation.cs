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

        public static void SetReadOnly(){
            var helperAutomation = new HelperAutomation();
            SetReadOnly(helperAutomation.GetFocusControlHandle());
        }

        public static void SetReadOnly(IntPtr windowHandle){
            const int EM_SETREADONLY = 0x00CF;
            Win32Declares.Message.SendMessage(windowHandle, EM_SETREADONLY, 1, 0);
        }

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

        public static bool FocusWindow(string caption) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, caption);
            return FocusWindow(findWindow);
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

        public static bool ResizeWindow(IntPtr window, Size newSize){
            Win32Types.RECT lpRect;
            Win32Declares.Rect.GetWindowRect(window, out lpRect);
            Rectangle rectangle = lpRect.ToRectangle();
            if (newSize.Width!=rectangle.Width||newSize.Height!=rectangle.Height){
                bool moveWindow = Win32Declares.Window.MoveWindow(window, rectangle.X, rectangle.Y, newSize.Width, newSize.Height, true);
                Application.DoEvents();
                return moveWindow;
            }
            return true;
        }

        public static bool MoveWindow(IntPtr window, Point newLocation){
            Win32Types.RECT lpRect;
            Win32Declares.Rect.GetWindowRect(window, out lpRect);
            Rectangle rectangle = lpRect.ToRectangle();
            bool moveWindow = Win32Declares.Window.MoveWindow(window, newLocation.X, newLocation.Y, rectangle.Width,rectangle.Height, true);
            Application.DoEvents();
            return moveWindow;
        }

        public static bool MoveWindow(string windowCaption, Point newLocation) {
            var findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
            return MoveWindow(findWindow, newLocation);
        }

        public static void MinimizeWindow(IntPtr handle) {
            Win32Declares.Window.ShowWindow(handle, Win32Declares.Window.ShowWindowEnum.SW_MINIMIZE);
        }

        public static void MaximizeWindow(IntPtr handle) {
            Win32Declares.Window.ShowWindow(handle, Win32Declares.Window.ShowWindowEnum.SW_SHOWMAXIMIZED);
        }
        #region CloseWindow
        public static void CloseWindow(IntPtr windowHandle){
            Win32Declares.Message.SendMessage(windowHandle, (uint) Win32Constants.Standard.WM_SYSCOMMAND,
                (int) Win32Constants.Standard.SC_CLOSE, 0);
        }

        public static void CloseWindow(string windowCaption) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
            CloseWindow(findWindow);
        }
        #endregion

        public static bool KillFocus(){
            var helperAutomation = new HelperAutomation();
            var focusControlHandle = helperAutomation.GetFocusControlHandle();
            Win32Declares.Message.SendMessage(focusControlHandle,
                Win32Constants.Focus.WM_KILLFOCUS, IntPtr.Zero, IntPtr.Zero);
            return helperAutomation.GetFocusControlHandle()!=focusControlHandle;
        }
    }
}