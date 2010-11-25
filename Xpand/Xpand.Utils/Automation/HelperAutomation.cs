using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    public class HelperAutomation {
        private ArrayList allWindowCaptions;
        private ArrayList allWindowHandles;


        public ArrayList AllWindowCaptions {
            get { return allWindowCaptions; }
            set { allWindowCaptions = value; }
        }

        public ArrayList AllWindowHandles {
            get { return allWindowHandles; }
            set { allWindowHandles = value; }
        }


        public void FindAllWindows(string parentWindowCaption) {
            allWindowCaptions = new ArrayList();
            allWindowHandles = new ArrayList();
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
            allWindowCaptions.Add(value);
            allWindowHandles.Add(hwnd);
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

        public IntPtr GetFocusControlHandle() {
#pragma warning disable 612,618
            var thisThreadID = new IntPtr(AppDomain.GetCurrentThreadId());
#pragma warning restore 612,618
            IntPtr activeHwnd = Win32Declares.WindowFocus.GetForegroundWindow();
            IntPtr focusedHwnd = IntPtr.Zero;
            if (activeHwnd != IntPtr.Zero) {
                IntPtr activeThreadID = Win32Declares.Process.GetWindowThreadProcessId(activeHwnd, IntPtr.Zero);
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