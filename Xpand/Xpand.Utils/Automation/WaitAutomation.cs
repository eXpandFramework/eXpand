using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    public class WaitAutomation {
        #region delegates
        #region Nested type: findWindowsWithTimeoutDelegate
        private delegate bool findWindowsWithTimeoutDelegate(HelperAutomation helperAutomation, string windowCaption);
        #endregion
        #region Nested type: waitForWindowToBeDisableDelegate
        private delegate bool waitForWindowToBeDisableDelegate(string windowCaption, string parentWindowCaption);
        #endregion
        #region Nested type: waitForWindowToCloseDelegate
        private delegate bool waitForWindowToCloseDelegate(string windowCaption);
        #endregion
        #region Nested type: waitForWindowToHaveTextDelegate
        private delegate bool waitForWindowToHaveTextDelegate(string text, IntPtr windowHandle, bool partialMatch);
        #endregion
        #endregion
        #region wait for window to ....
        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="windowCaption"></param>
        /// <returns>0 if window not found</returns>
        public static IntPtr WaitForWindowToOpen(string windowCaption) {
            var helperAutomation = new HelperAutomation();

            if (findWindowsWithTimeout(helperAutomation, windowCaption)) {
                IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
                return findWindow;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="windowCaption"></param>
        /// <returns>0 if window not found</returns>
        public static IntPtr WaitForWindowToBeFocused(string windowCaption) {
            if (findFocusedWindowsWithTimeout(windowCaption)) {
                IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
                return findWindow;
            }
            return IntPtr.Zero;
        }

        public static void WaitFor(int milliSec) {
            Wait.SleepFor(milliSec);
        }
        #region WaitForWindowToBeDisable
        private static bool waitForWindowToBeDisableHandler(string windowCaption, string parentWindowCaption) {
            IntPtr findWindowParent = Win32Declares.WindowHandles.FindWindow(null, parentWindowCaption);
            IntPtr findWindowEx = Win32Declares.WindowHandles.FindWindowEx(findWindowParent, IntPtr.Zero, null,
                                                                           windowCaption);
            while (true) {
                bool enabled = Win32Declares.Window.IsWindowEnabled(findWindowEx);
                if (!enabled)
                    return true;
            }
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="windowCaption"></param>
        /// <param name="parentWindowCaption"></param>
        /// <returns></returns>
        public static bool WaitForWindowToBeDisable(string windowCaption, string parentWindowCaption) {
            waitForWindowToBeDisableDelegate waitForWindowToBeDisableDelegate = waitForWindowToBeDisableHandler;
            IAsyncResult asyncResult = waitForWindowToBeDisableDelegate.BeginInvoke(windowCaption, parentWindowCaption,
                                                                                    null, null);
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne(5000, false);
            return asyncResult.IsCompleted ? waitForWindowToBeDisableDelegate.EndInvoke(asyncResult) : false;
        }
        #endregion
        #region WaitForWindowToHaveText
        private static bool waitForWindowToHaveTextHandler(string text, IntPtr windowHandle, bool partialMatch) {
            while (true) {
                if (!partialMatch) {
                    if (InteractivityAutomation.GetText(windowHandle) == text)
                        return true;
                } else {
                    if (InteractivityAutomation.GetText(windowHandle).IndexOf(text) > -1)
                        return true;
                }
            }
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="text"></param>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static bool WaitForWindowToHaveText(string text, IntPtr windowHandle, bool partialMatch) {
            waitForWindowToHaveTextDelegate waitForWindowToHaveTextDelegate = waitForWindowToHaveTextHandler;
            IAsyncResult asyncResult = waitForWindowToHaveTextDelegate.BeginInvoke(text, windowHandle, partialMatch,
                                                                                   null, null);
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne(5000, false);
            bool b = asyncResult.IsCompleted ? waitForWindowToHaveTextDelegate.EndInvoke(asyncResult) : false;
            return b;
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="text"></param>
        /// <param name="windowPosition"></param>
        /// <returns></returns>
        public static bool WaitForWindowToHaveText(string text, Point windowPosition) {
            IntPtr intPtr = new HelperAutomation().WindowFromPoint(windowPosition);
            return WaitForWindowToHaveText(text, intPtr, false);
        }
        #endregion
        #region WaitForWindowToClose
        private static bool waitForWindowToCloseHandler(string windowCaption) {
            while (true) {
                IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
                if (findWindow == IntPtr.Zero)
                    return true;
            }
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="windowCaption"></param>
        public static bool WaitForWindowToClose(string windowCaption) {
            waitForWindowToCloseDelegate waitForWindowToCloseDelegate = waitForWindowToCloseHandler;
            IAsyncResult asyncResult = waitForWindowToCloseDelegate.BeginInvoke(windowCaption, null, null);
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne(5000, false);
            bool b = asyncResult.IsCompleted ? waitForWindowToCloseDelegate.EndInvoke(asyncResult) : false;
            return b;
        }
        #endregion
        #region private methods
        private static bool findWindowsWithTimeoutHandler(HelperAutomation helperAutomation, string windowCaption) {
            while (true) {
                helperAutomation.FindAllWindows(null);
                if (helperAutomation.AllWindowCaptions.Contains(windowCaption))
                    return true;
            }
        }

        private static bool findWindowsWithTimeout(HelperAutomation helperAutomation, string windowCaption) {
            findWindowsWithTimeoutDelegate findWindowsWithTimeoutDelegate = findWindowsWithTimeoutHandler;
            IAsyncResult asyncResult = findWindowsWithTimeoutDelegate.BeginInvoke(helperAutomation, windowCaption, null,
                                                                                  null);
            if (!asyncResult.IsCompleted)
                asyncResult.AsyncWaitHandle.WaitOne(5000, false);
            return asyncResult.IsCompleted ? findWindowsWithTimeoutDelegate.EndInvoke(asyncResult) : false;
        }

        private static bool findFocusedWindowsWithTimeout(string windowCaption) {
            TimeSpan timeSpan = DateTime.Now.TimeOfDay;
            while (timeSpan.Subtract(DateTime.Now.TimeOfDay).TotalSeconds > -5) {
                Application.DoEvents();
                IntPtr findWindow = Win32Declares.WindowHandles.FindWindow(null, windowCaption);
                if (!findWindow.Equals(IntPtr.Zero) && WindowAutomation.FocusedWindowCaption == windowCaption)
                    return true;
            }
            return false;
        }
        #endregion
        #endregion
        #region WaitForFileToBeCreated
        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string directoryPath, string filter) {
            return WaitForFileToBeCreated(directoryPath, 5000, filter);
        }

        /// <summary>
        /// default timeout is 5000 millisec
        /// </summary>
        /// <param name="filePath">path and name of the file</param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string filePath) {
            return WaitForFileToBeCreated(filePath, 5000);
        }

        /// <summary>
        /// _
        /// </summary>
        /// <param name="filePath">path and name of the file</param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string filePath, int milliSec) {
            return WaitForFileToBeCreated(Path.GetDirectoryName(filePath), milliSec, Path.GetFileName(filePath));
        }


        /// <summary>
        /// _
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>true if the file exists at the given time</returns>
        public static bool WaitForFileToBeCreated(string directoryPath, int milliSec, string filter) {
            var fileAutomation = new FileAutomation();
            WaitForChangedResult waitForChanged = fileAutomation.WaitForChanged(directoryPath, filter,
                                                                                WatcherChangeTypes.Created, milliSec);
            bool b = waitForChanged.Name + "" != "";
            return b;
        }
        #endregion
    }
}