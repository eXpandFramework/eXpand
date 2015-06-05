using System;
using System.Drawing;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    public static class WindowAutomation {
        static readonly IntPtr _desktopHandle = Win32Declares.WindowHandles.GetDesktopWindow();
        [SecurityCritical]
        public static Image GetScreenshot(this IntPtr handle) {
            Image result;
            if (handle != IntPtr.Zero) {

                if (handle != _desktopHandle) {
                    Win32Declares.WindowFocus.SetForegroundWindow(handle);
                    Thread.Sleep(1000);
                }
                var hdcSrc = Win32Declares.WindowHandles.GetWindowDC(_desktopHandle);

                Win32Types.RECT windowRect;
                Win32Declares.Rect.GetWindowRect(handle, out windowRect);

                var width = windowRect.Right - windowRect.Left;
                var height = windowRect.Bottom - windowRect.Top;

                if (handle != _desktopHandle) {
                    Win32Declares.Window.SetWindowPos(handle, IntPtr.Zero, 0, 0, width, height, 0);
                }

                
                Win32Declares.Rect.GetWindowRect(handle, out windowRect);

                width = windowRect.Right - windowRect.Left;
                height = windowRect.Bottom - windowRect.Top;
                
                
                var hdcDest = Win32Declares.GDI32.CreateCompatibleDC(hdcSrc);
                var hBitmap = Win32Declares.GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
                var hOld = Win32Declares.GDI32.SelectObject(hdcDest, hBitmap);
                Win32Declares.GDI32.BitBlt(hdcDest, -windowRect.Left, -windowRect.Top, windowRect.Right, windowRect.Bottom,
                    hdcSrc, 0, 0, Win32Constants.TernaryRasterOperations.SRCCOPY);
                Win32Declares.GDI32.SelectObject(hdcDest, hOld);
                Win32Declares.GDI32.DeleteDC(hdcDest);
                Win32Declares.GDI32.ReleaseDC(_desktopHandle, hdcSrc);
                result = Image.FromHbitmap(hBitmap);
                Win32Declares.GDI32.DeleteObject(hBitmap);
            }
            else {
                result = GetScreenshot(_desktopHandle);
            }
            return result;
        }

        public static string WindowText(this IntPtr intPtr) {
            int length = Win32Declares.Window.GetWindowTextLength(intPtr);
            var sb = new StringBuilder(length + 1);
            Win32Declares.Window.GetWindowText(intPtr, sb, sb.Capacity);
            return sb.ToString();
        }

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


        public static bool ForceWindowToForeground(IntPtr windowHandle){
            if (Win32Declares.WindowFocus.GetForegroundWindow() == windowHandle)
                return true;
            HelperAutomation.AttachedThreadInputAction(
                () => {
                    
                    Win32Declares.WindowFocus.BringWindowToTop(windowHandle);
                    var showWindowEnum = Win32Declares.Window.IsIconic(windowHandle)? Win32Declares.Window.ShowWindowEnum.SW_RESTORE: Win32Declares.Window.ShowWindowEnum.SW_SHOW;
                    Win32Declares.Window.ShowWindow(windowHandle, showWindowEnum);
                });
            return Win32Declares.WindowFocus.GetForegroundWindow() == windowHandle;
        }

        public static bool ForceWindowToForeground(string caption) {
            IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, caption);
            return ForceWindowToForeground(findWindow);
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