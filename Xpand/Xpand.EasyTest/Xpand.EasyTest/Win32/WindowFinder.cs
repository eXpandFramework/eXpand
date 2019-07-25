using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Xpand.EasyTest.Win32{
    public class WindowFinder {
        // Win32 constants.
        const int WmGettext = 0x000D;
        const int WmGettextlength = 0x000E;

        // Win32 functions that have all been used in previous blogs.
        [DllImport("User32.Dll")]
        private static extern void GetClassName(int hWnd, StringBuilder s, int nMaxCount);

        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(int hWnd, int msg, int wParam, StringBuilder lParam);

        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(int hWnd, int msg, int wParam, int lParam);

        [DllImport("user32")]
        private static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        // EnumChildWindows works just like EnumWindows, except we can provide a parameter that specifies the parent
        // window handle. If this is NULL or zero, it works just like EnumWindows. Otherwise it'll only return windows
        // whose parent window handle matches the hWndParent parameter.
        [DllImport("user32.Dll")]
        private static extern Boolean EnumChildWindows(int hWndParent, PChildCallBack lpEnumFunc, int lParam);

        // The PChildCallBack delegate that we used with EnumWindows.
        private delegate bool PChildCallBack(int hWnd, int lParam);

        // This is an event that is run each time a window was found that matches the search criterias. The boolean
        // return value of the delegate matches the functionality of the PChildCallBack delegate function.
        private event FoundWindowCallback FoundWindow;
        public delegate bool FoundWindowCallback(int hWnd);

        // Members that'll hold the search criterias while searching.
        private Regex _className;
        private Regex _windowText;
        private Regex _process;

        // The main search function of the WindowFinder class. The parentHandle parameter is optional, taking in a zero if omitted.
        // The className can be null as well, in this case the class name will not be searched. For the window text we can input
        // a Regex object that will be matched to the window text, unless it's null. The process parameter can be null as well,
        // otherwise it'll match on the process name (Internet Explorer = "iexplore"). Finally we take the FoundWindowCallback
        // function that'll be called each time a suitable window has been found.
        public void FindWindows(int parentHandle, Regex className, Regex windowText, Regex process, FoundWindowCallback fwc) {
            _className = className;
            _windowText = windowText;
            _process = process;

            // Add the FounWindowCallback to the foundWindow event.
            FoundWindow = fwc;

            // Invoke the EnumChildWindows function.
            EnumChildWindows(parentHandle, EnumChildWindowsCallback, 0);
        }

        // This function gets called each time a window is found by the EnumChildWindows function. The foun windows here
        // are NOT the final found windows as the only filtering done by EnumChildWindows is on the parent window handle.
        private bool EnumChildWindowsCallback(int handle, int lParam) {
            // If a class name was provided, check to see if it matches the window.
            if (_className != null) {
                StringBuilder sbClass = new StringBuilder(256);
                GetClassName(handle, sbClass, sbClass.Capacity);

                // If it does not match, return true so we can continue on with the next window.
                if (!_className.IsMatch(sbClass.ToString()))
                    return true;
            }

            // If a window text was provided, check to see if it matches the window.
            if (_windowText != null) {
                int txtLength = SendMessage(handle, WmGettextlength, 0, 0);
                StringBuilder sbText = new StringBuilder(txtLength + 1);
                SendMessage(handle, WmGettext, sbText.Capacity, sbText);

                // If it does not match, return true so we can continue on with the next window.
                if (!_windowText.IsMatch(sbText.ToString()))
                    return true;
            }

            // If a process name was provided, check to see if it matches the window.
            if (_process != null) {
                int processID;
                GetWindowThreadProcessId(handle, out processID);

                // Now that we have the process ID, we can use the built in .NET function to obtain a process object.
                Process p = Process.GetProcessById(processID);

                // If it does not match, return true so we can continue on with the next window.
                if (!_process.IsMatch(p.ProcessName))
                    return true;
            }

            // If we get to this point, the window is a match. Now invoke the foundWindow event and based upon
            // the return value, whether we should continue to search for windows.
            Debug.Assert(FoundWindow != null, "FoundWindow != null");
            return FoundWindow(handle);
        }
    }
}
