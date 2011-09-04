using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    public class InteractivityAutomation {
        #region MouseMovementEnum enum
        public enum MouseMovementEnum {
            RelativeToScreen
        }
        #endregion
        #region PasteFromClipBoard
        /// <summary>
        /// paste clipbard data
        /// </summary>
        /// <param name="data">the data to be pasted</param>
        /// <param name="mainWindowHandle">the widbow that contains the control in which the data is going to be pasted</param>
        public static void PasteFromClipBoard(object data, IntPtr mainWindowHandle) {
            Clipboard.SetDataObject(data, false);
            PasteFromClipBoard(mainWindowHandle);
        }

        /// <summary>
        /// paste clipbard data
        /// </summary>
        /// <param name="mainWindowHandle">the widbow that contains the control in which the data is going to be pasted</param>
        public static void PasteFromClipBoard(IntPtr mainWindowHandle) {
            WindowAutomation.FocusWindow(mainWindowHandle);
            var helperAutomation = new HelperAutomation();
            IntPtr focusControlHandle = helperAutomation.GetFocusControlHandle();
            Win32Declares.Message.SendMessage(focusControlHandle, Win32Constants.Clipboard.WM_PASTE, IntPtr.Zero,
                                              IntPtr.Zero);
        }
        #endregion
        public static void ClickButton(IntPtr intPtr) {
            Win32Declares.Message.PostMessage(intPtr, Win32Constants.Button.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        }
        #region ClickMenu
        public static void ClickMenu(IntPtr windowHandle, int[] menuPos) {
            IntPtr menuRoot = Win32Declares.Menu.GetMenu(windowHandle);
            for (int i = 0; i < menuPos.Length - 1; i++) {
                menuRoot = Win32Declares.Menu.GetSubMenu(menuRoot, menuPos[i]);
            }
            int menuItemId = Win32Declares.Menu.GetMenuItemID(menuRoot, menuPos[menuPos.Length - 1]);
            Win32Declares.Message.PostMessage(windowHandle, Win32Constants.Standard.WM_COMMAND, menuItemId, 0);
        }

        public static void ClickMenu(string applicationCaption, int[] menuPos) {
            IntPtr windowHandle = Win32Declares.WindowHandles.FindWindow(null, applicationCaption);
            ClickMenu(windowHandle, menuPos);
        }
        #endregion
        #region mouse
        public static void MoveMouse(Point point, MouseMovementEnum mouseMovementEnum) {
            Cursor.Position = point;
            Application.DoEvents();
        }

        public static void ClickMouse(MouseButtons mouseButtons, Point point) {
            if (mouseButtons != MouseButtons.Left)
                throw new NotImplementedException();
            MoveMouse(point, MouseMovementEnum.RelativeToScreen);
            var input = new Win32Types.INPUT {
                Type = Win32Types.INPUTTYPE.INPUT_MOUSE,
                mi = {
                    dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_LEFTDOWN,
                    mouseData = 0,
                    time = 0,
                    dwExtraInfo = IntPtr.Zero
                }
            };

            Win32Declares.KeyBoard.SendInput(1, new[] { input }, Win32Types.MOUSEINPUT.cbSize);
            input.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_LEFTUP;
            Win32Declares.KeyBoard.SendInput(1, new[] { input }, Win32Types.MOUSEINPUT.cbSize);
            Application.DoEvents();
        }
        #endregion
        #region SetText
        /// <summary>
        /// finds the focused control and sets its text
        /// </summary>
        /// <param name="text"></param>
        public static void SetText(string text) {
            var automation = new HelperAutomation();
            IntPtr focusControlHandle = automation.GetFocusControlHandle();
            SetText(focusControlHandle, text);
        }

        public static void SetText(IntPtr handle, string text) {
            Win32Declares.Message.SendMessage(handle, Win32Constants.Standard.WM_SETTEXT, IntPtr.Zero, text);
            Application.DoEvents();
        }

        public static void SetText(Point point, string textToSet) {
            var helperAutomation = new HelperAutomation();
            IntPtr windowFromPoint = helperAutomation.WindowFromPoint(point);
            SetText(windowFromPoint, textToSet);
        }
        #endregion
        #region GetText
        public static string GetText(IntPtr handle) {
            int windowTextLenght =
                Win32Declares.Message.SendMessage(handle, Win32Constants.Standard.WM_GETTEXTLENGTH, IntPtr.Zero,
                                                  IntPtr.Zero).ToInt32();
            var stringBuilder = new StringBuilder(windowTextLenght + 100);
            Win32Declares.Message.SendMessage(handle, Win32Constants.Standard.WM_GETTEXT,
                                              (IntPtr)stringBuilder.Capacity, stringBuilder);
            string s = stringBuilder.ToString();
            return s;
        }

        public static string GetText(Point point) {
            var helperAutomation = new HelperAutomation();
            IntPtr windowFromPoint = helperAutomation.WindowFromPoint(point);
            return GetText(windowFromPoint);
        }

        public static string[] GetListBoxItems(IntPtr handle) {
            var lCount =
                (int)
                Win32Declares.Message.SendMessage(handle, Win32Constants.ListBox.LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            var listBoxItems = new string[lCount];
            for (int i = 0; i < lCount; i++) {
                var lenght =
                    (int)
                    Win32Declares.Message.SendMessage(handle, Win32Constants.ListBox.LB_GETTEXTLEN, (IntPtr)i,
                                                      IntPtr.Zero);
                var stringBuilder = new StringBuilder(lenght);
                Win32Declares.Message.SendMessage(handle, Win32Constants.ListBox.LB_GETTEXT, (IntPtr)i, stringBuilder);
                listBoxItems[i] = stringBuilder.ToString();
            }
            return listBoxItems;
        }
        #endregion
        #region SendKeys
        public static void SendKeys(Keys keys) {
            var keyEventArgs = new KeyEventArgs(keys);
            Enum.Parse(typeof(Win32Constants.VirtualKeys), keyEventArgs.KeyCode.ToString());
            SendKeys(keyEventArgs.Control, keyEventArgs.Alt, keyEventArgs.Shift,
                     (Win32Constants.VirtualKeys)keyEventArgs.KeyValue);
        }

        public static void SendKeys(string s) {
            s = s.ToUpper();
            for (int i = 0; i < s.Length; i++) {
                var parse =
                    (Win32Constants.VirtualKeys)Enum.Parse(typeof(Win32Constants.VirtualKeys), s.Substring(i, 1));
                SendKeys(false, false, false, parse);
            }
        }

        public static void SendKeys(bool control, bool alt, bool shift, Win32Constants.VirtualKeys keys) {
            if (control)
                pressKey(Win32Constants.VirtualKeys.ControlLeft);
            if (alt)
                pressKey(Win32Constants.VirtualKeys.Menu);
            if (shift)
                pressKey(Win32Constants.VirtualKeys.ShiftLeft);

            sendKey(keys);

            if (control)
                releaseKey(Win32Constants.VirtualKeys.ControlLeft);
            if (alt)
                releaseKey(Win32Constants.VirtualKeys.Menu);
            if (shift)
                releaseKey(Win32Constants.VirtualKeys.ShiftLeft);

            Application.DoEvents();
        }

        private static void sendKey(Win32Constants.VirtualKeys keys) {
            pressKey(keys);
            releaseKey(keys);
        }

        private static void releaseKey(Win32Constants.VirtualKeys keys) {
            var input = new Win32Types.INPUT {
                Type = Win32Types.INPUTTYPE.INPUT_KEYBOARD,
                ki = {
                    wVk = keys,
                    dwFlags = Win32Constants.KeyboardEvent.KEYEVENTF_KEYUP
                }
            };
            Win32Declares.KeyBoard.SendInput(1, new[] { input }, Win32Types.KEYBDINPUT.cbSize);
        }

        private static void pressKey(Win32Constants.VirtualKeys keys) {
            var input = new Win32Types.INPUT {
                Type = Win32Types.INPUTTYPE.INPUT_KEYBOARD,
                ki = {
                    wVk = keys,
                    dwFlags = Win32Constants.KeyboardEvent.KEYEVENTF_EXTENDEDKEY
                }
            };
            Win32Declares.KeyBoard.SendInput(1, new[] { input }, Win32Types.KEYBDINPUT.cbSize);
        }
        #endregion
        #region printer methods
        public static bool SetDefaultPrinter(string printerName) {
            if (Environment.OSVersion.Platform == PlatformID.Win32Windows) {
                if (printerName.IndexOf(",") > -1)
                    printerName = printerName.Split(',')[0];
                var returnedString = new StringBuilder(1024);
                Win32Declares.IniFiles.GetProfileString("PrinterPorts", printerName, "", returnedString, 1024);
                string[] split = returnedString.ToString().Split(',');
                string printer = "device=" + printerName + "," + split[0] + "," + split[1];
                string fileName = Path.Combine(Environment.SystemDirectory.ToLower().Replace("system", ""), "win.ini");
                TextReader textReader = new StreamReader(fileName);
                string readToEnd = textReader.ReadToEnd();
                Match matchResults = Regex.Match(readToEnd, "device=.*");
                if (matchResults.Success) {
                    textReader.Close();
                    string value = readToEnd.Substring(0, matchResults.Index) + printer.Trim() +
                                   readToEnd.Substring(matchResults.Index + matchResults.Length);
                    TextWriter textWriter = new StreamWriter(fileName);
                    textWriter.Write(value);
                    textWriter.Close();
                    var lParam = new StringBuilder();
                    lParam.Append("windows");
                    Win32Declares.Message.SendNotifyMessage(Win32Constants.BroadCast.HWND_BROADCAST,
                                                            Win32Constants.BroadCastMessages.WM_WININICHANGE,
                                                            UIntPtr.Zero, lParam);
                    return true;
                }
                return false;
            }

            bool retprinter = Win32Declares.Printers.SetDefaultPrinter(printerName);
            return retprinter;
        }

        public static string GetDefaultPrinter() {
            if (Environment.OSVersion.Platform == PlatformID.Win32Windows) {
                string fileName = Path.Combine(Environment.SystemDirectory.ToLower().Replace("system", ""), "win.ini");
                TextReader textReader = new StreamReader(fileName);
                string printer = Regex.Match(textReader.ReadToEnd(), "device=.*").Value.Split('=')[1].TrimEnd();
                textReader.Close();
                return printer;
            }
            var builder = new StringBuilder(256);
            int capacity = builder.Capacity;
            Win32Declares.Printers.GetDefaultPrinter(builder, ref capacity);
            return builder.ToString();
        }
        #endregion
    }
}