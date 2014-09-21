using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    [SecuritySafeCritical]
    public class HelperAutomation {
        private ICollection<string> _allWindowCaptions;
        private ICollection<IntPtr> _allWindowHandles;


        public ICollection<string> AllWindowCaptions {
            get { return _allWindowCaptions; }
        }

        public ICollection<IntPtr> AllWindowHandles {
            get { return _allWindowHandles; }
        }


        public void FindAllWindows(string parentWindowCaption) {
            _allWindowCaptions = new Collection<string>();
            _allWindowHandles = new Collection<IntPtr>();
            IntPtr windowHandle = Win32Declares.WindowHandles.FindWindow(null, parentWindowCaption);
            if (parentWindowCaption == null)
                windowHandle = IntPtr.Zero;
            Win32Declares.Window.EnumChildWindows(windowHandle, OnEnumWindow, IntPtr.Zero);
        }

        private int OnEnumWindow(IntPtr hwnd, IntPtr lParam) {
            int length = Win32Declares.Window.GetWindowTextLength(hwnd);
            var sb = new StringBuilder(length + 1);
            Win32Declares.Window.GetWindowText(hwnd, sb, sb.Capacity);
            string value = sb.ToString();
            _allWindowCaptions.Add(value);
            _allWindowHandles.Add(hwnd);
            return 1;
        }

        /// <summary>
        /// Finds the depest visible window that contain the specified point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IntPtr WindowFromPoint(Point point) {
            var pointW = new Win32Types.POINT(point.X, point.Y);
            IntPtr parentWindowFromPoint = Win32Declares.Window.WindowFromPoint(pointW);
            IntPtr childWindowFromPointEx = Win32Declares.Window.ChildWindowFromPointEx(parentWindowFromPoint, pointW,
                                                                                        Win32Constants.
                                                                                            ChildWindowFromPointFlags.
                                                                                            CWP_SKIPINVISIBLE);
            if (childWindowFromPointEx == IntPtr.Zero) {
                return parentWindowFromPoint;
            }
            return childWindowFromPointEx;
        }

        public static void AttachedThreadInputAction(Action action) {
            var foreThread = Win32Declares.Process.GetWindowThreadProcessId(Win32Declares.WindowFocus.GetForegroundWindow(), IntPtr.Zero);
            var appThread = Win32Declares.Thread.GetCurrentThreadId();
            bool threadsAttached = false;

            try {
                threadsAttached =
                    foreThread == appThread || Win32Declares.Thread.AttachThreadInput(foreThread, appThread, true);

                if (threadsAttached) action();
                else throw new ThreadStateException("AttachThreadInput failed." + Marshal.GetLastWin32Error());
            }
            finally {
                if (threadsAttached)
                    Win32Declares.Thread.AttachThreadInput(foreThread, appThread, false);
            }
        }

        public IntPtr GetFocusControlHandle() {
#pragma warning disable 612,618
            var thisThreadID = (uint) AppDomain.GetCurrentThreadId();
#pragma warning restore 612,618
            IntPtr activeHwnd = Win32Declares.WindowFocus.GetForegroundWindow();
            IntPtr focusedHwnd = IntPtr.Zero;
            if (activeHwnd != IntPtr.Zero) {
                uint activeThreadID = Win32Declares.Process.GetWindowThreadProcessId(activeHwnd, IntPtr.Zero);
                if (Win32Declares.Thread.AttachThreadInput(activeThreadID, thisThreadID, true)) {
                    focusedHwnd = Win32Declares.WindowFocus.GetFocus();
                    Win32Declares.Thread.AttachThreadInput(activeThreadID, thisThreadID, false);
                } else
                    focusedHwnd = Win32Declares.WindowFocus.GetFocus();
            }

            return focusedHwnd;
        }

        public void KillProcesses(string processName) {
            Process[] processesByName = Process.GetProcessesByName(processName);
            foreach (Process process in processesByName) {
                process.Kill();
            }
        }
    }
}