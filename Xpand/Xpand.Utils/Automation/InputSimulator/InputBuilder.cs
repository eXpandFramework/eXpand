using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation.InputSimulator{
    internal class InputBuilder : IEnumerable<Win32Types.INPUT>{
        private readonly List<Win32Types.INPUT> _inputList;

        public InputBuilder(){
            _inputList = new List<Win32Types.INPUT>();
        }

        public Win32Types.INPUT this[int position]{
            get { return _inputList[position]; }
        }

        public IEnumerator<Win32Types.INPUT> GetEnumerator(){
            return _inputList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator(){
            return GetEnumerator();
        }

        public Win32Types.INPUT[] ToArray(){
            return _inputList.ToArray();
        }

        public static bool IsExtendedKey(Win32Constants.VirtualKeys keyCode){
            if (keyCode == Win32Constants.VirtualKeys.Menu ||
                keyCode == Win32Constants.VirtualKeys.LMenu ||
                keyCode == Win32Constants.VirtualKeys.RMenu ||
                keyCode == Win32Constants.VirtualKeys.Control ||
                keyCode == Win32Constants.VirtualKeys.ControlRight ||
                keyCode == Win32Constants.VirtualKeys.Insert ||
                keyCode == Win32Constants.VirtualKeys.Delete ||
                keyCode == Win32Constants.VirtualKeys.Home ||
                keyCode == Win32Constants.VirtualKeys.End ||
                keyCode == Win32Constants.VirtualKeys.Prior ||
                keyCode == Win32Constants.VirtualKeys.Next ||
                keyCode == Win32Constants.VirtualKeys.Right ||
                keyCode == Win32Constants.VirtualKeys.Up ||
                keyCode == Win32Constants.VirtualKeys.Left ||
                keyCode == Win32Constants.VirtualKeys.Down ||
                keyCode == Win32Constants.VirtualKeys.NumLock ||
                keyCode == Win32Constants.VirtualKeys.Cancel ||
                keyCode == Win32Constants.VirtualKeys.Snapshot ||
                keyCode == Win32Constants.VirtualKeys.Divide){
                return true;
            }
            return false;
        }

        public InputBuilder AddKeyDown(Win32Constants.VirtualKeys keyCode){
            var down = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_KEYBOARD,};
            var keybdinput = new Win32Types.KEYBDINPUT{
                wVk = keyCode,
                wScan = 0,
                dwFlags = IsExtendedKey(keyCode) ? Win32Constants.KeyboardEvent.KEYEVENTF_EXTENDEDKEY : 0,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            };
            down.mkhi.ki = keybdinput;
            _inputList.Add(down);
            return this;
        }

        public InputBuilder AddKeyUp(Win32Constants.VirtualKeys keyCode){
            var up =
                new Win32Types.INPUT{
                    type = Win32Types.INPUTTYPE.INPUT_KEYBOARD,
//                    Data ={
//                        Keyboard =
//                            new Win32Types.KEYBDINPUT{
//                                KeyCode = (UInt16) keyCode,
//                                Scan = 0,
//                                Flags = (UInt32) (IsExtendedKey(keyCode)
//                                    ? KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey
//                                    : KeyboardFlag.KeyUp),
//                                Time = 0,
//                                ExtraInfo = IntPtr.Zero
//                            }
//                    }
                };
            var keybdinput = new Win32Types.KEYBDINPUT{
                wVk = keyCode,
                wScan = 0,
                dwFlags =
                    IsExtendedKey(keyCode)
                        ? Win32Constants.KeyboardEvent.KEYEVENTF_KEYUP |
                          Win32Constants.KeyboardEvent.KEYEVENTF_EXTENDEDKEY
                        : Win32Constants.KeyboardEvent.KEYEVENTF_KEYUP,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            };
            up.mkhi.ki = keybdinput;
            _inputList.Add(up);
            return this;
        }

        public InputBuilder AddKeyPress(Win32Constants.VirtualKeys keyCode){
            AddKeyDown(keyCode);
            AddKeyUp(keyCode);
            return this;
        }

        public InputBuilder AddCharacter(char character){
            UInt16 scanCode = character;

            var down = new Win32Types.INPUT{
                type = Win32Types.INPUTTYPE.INPUT_KEYBOARD,
            };
            var keybdinput = new Win32Types.KEYBDINPUT{
                wVk = 0,
                wScan = scanCode,
                dwFlags = Win32Constants.KeyboardEvent.KEYEVENTF_UNICODE,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            };

            down.mkhi.ki = keybdinput;
            var up = new Win32Types.INPUT{
                type = Win32Types.INPUTTYPE.INPUT_KEYBOARD,
//                Data ={
//                    Keyboard =
//                        new Win32Types.KEYBDINPUT{
//                            KeyCode = 0,
//                            Scan = scanCode,
//                            Flags =
//                                (UInt32) (KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
//                            Time = 0,
//                            ExtraInfo = IntPtr.Zero
//                        }
//                }
            };
            keybdinput = new Win32Types.KEYBDINPUT{
                wVk = 0,
                wScan = scanCode,
                dwFlags = Win32Constants.KeyboardEvent.KEYEVENTF_KEYUP | Win32Constants.KeyboardEvent.KEYEVENTF_UNICODE,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            };
            up.mkhi.ki = keybdinput;
            // Handle extended keys:
            // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
            // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
            if ((scanCode & 0xFF00) == 0xE000){
                down.mkhi.ki.dwFlags |= Win32Constants.KeyboardEvent.KEYEVENTF_EXTENDEDKEY;
                up.mkhi.ki.dwFlags |= Win32Constants.KeyboardEvent.KEYEVENTF_EXTENDEDKEY;
            }

            _inputList.Add(down);
            _inputList.Add(up);
            return this;
        }

        public InputBuilder AddCharacters(IEnumerable<char> characters){
            foreach (char character in characters){
                AddCharacter(character);
            }
            return this;
        }

        public InputBuilder AddCharacters(string characters){
            return AddCharacters(characters.ToCharArray());
        }

        public InputBuilder AddRelativeMouseMovement(int x, int y){
            var movement = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            movement.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_MOVE;
            movement.mkhi.mi.dx = x;
            movement.mkhi.mi.dy = y;

            _inputList.Add(movement);

            return this;
        }


        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);

        private int CalculateAbsoluteCoordinateX(int x){
            return (x*65536)/GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        private int CalculateAbsoluteCoordinateY(int y){
            return (y*65536)/GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }

        public InputBuilder AddAbsoluteMouseMovement(int absoluteX, int absoluteY){
            var movement = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            movement.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_MOVE |
                                       Win32Constants.MouseEvent.MOUSEEVENTF_ABSOLUTE;
            movement.mkhi.mi.dx = CalculateAbsoluteCoordinateX(absoluteX);
            movement.mkhi.mi.dy = CalculateAbsoluteCoordinateY(absoluteY);

            _inputList.Add(movement);

            return this;
        }

        public InputBuilder AddAbsoluteMouseMovementOnVirtualDesktop(int absoluteX, int absoluteY){
            var movement = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            movement.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_MOVE |
                                       Win32Constants.MouseEvent.MOUSEEVENTF_ABSOLUTE |
                                       Win32Constants.MouseEvent.MOUSEEVENTF_VIRTUALDESK;
            movement.mkhi.mi.dx = absoluteX;
            movement.mkhi.mi.dy = absoluteY;

            _inputList.Add(movement);

            return this;
        }

        public InputBuilder AddMouseButtonDown(MouseButton button){
            var buttonDown = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            buttonDown.mkhi.mi.dwFlags = ToMouseButtonDownFlag(button);

            _inputList.Add(buttonDown);

            return this;
        }

        public InputBuilder AddMouseXButtonDown(int xButtonId){
            var buttonDown = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            buttonDown.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_XDOWN;
            buttonDown.mkhi.mi.mouseData = xButtonId;
            _inputList.Add(buttonDown);

            return this;
        }

        public InputBuilder AddMouseButtonUp(MouseButton button){
            var buttonUp = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            buttonUp.mkhi.mi.dwFlags = ToMouseButtonUpFlag(button);

            _inputList.Add(buttonUp);

            return this;
        }

        public InputBuilder AddMouseXButtonUp(int xButtonId){
            var buttonUp = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            buttonUp.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_XUP;
            buttonUp.mkhi.mi.mouseData = xButtonId;
            _inputList.Add(buttonUp);

            return this;
        }

        public InputBuilder AddMouseButtonClick(MouseButton button){
            return AddMouseButtonDown(button).AddMouseButtonUp(button);
        }

        public InputBuilder AddMouseXButtonClick(int xButtonId){
            return AddMouseXButtonDown(xButtonId).AddMouseXButtonUp(xButtonId);
        }

        public InputBuilder AddMouseButtonDoubleClick(MouseButton button){
            return AddMouseButtonClick(button).AddMouseButtonClick(button);
        }

        public InputBuilder AddMouseXButtonDoubleClick(int xButtonId){
            return AddMouseXButtonClick(xButtonId).AddMouseXButtonClick(xButtonId);
        }

        public InputBuilder AddMouseVerticalWheelScroll(int scrollAmount){
            var scroll = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            scroll.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_VWHEEL;
            scroll.mkhi.mi.mouseData = scrollAmount;

            _inputList.Add(scroll);

            return this;
        }

        public InputBuilder AddMouseHorizontalWheelScroll(int scrollAmount){
            var scroll = new Win32Types.INPUT{type = Win32Types.INPUTTYPE.INPUT_MOUSE};
            scroll.mkhi.mi.dwFlags = Win32Constants.MouseEvent.MOUSEEVENTF_HWHEEL;
            scroll.mkhi.mi.mouseData = scrollAmount;

            _inputList.Add(scroll);

            return this;
        }

        private Win32Constants.MouseEvent ToMouseButtonDownFlag(MouseButton button){
            button = GetSwappedMouseButton(button);
            switch (button){
                case MouseButton.LeftButton:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_LEFTDOWN;

                case MouseButton.MiddleButton:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_MIDDLEDOWN;

                case MouseButton.RightButton:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_RIGHTDOWN;

                default:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_LEFTDOWN;
            }
        }

        private Win32Constants.MouseEvent ToMouseButtonUpFlag(MouseButton button){
            button = GetSwappedMouseButton(button);
            switch (button){
                case MouseButton.LeftButton:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_LEFTUP;

                case MouseButton.MiddleButton:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_MIDDLEUP;

                case MouseButton.RightButton:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_RIGHTUP;

                default:
                    return Win32Constants.MouseEvent.MOUSEEVENTF_LEFTUP;
            }
        }

        private MouseButton GetSwappedMouseButton(MouseButton button){
            if (System.Windows.Forms.SystemInformation.MouseButtonsSwapped) {
                if (button == MouseButton.LeftButton)
                    button = MouseButton.RightButton;
                else if (button == MouseButton.RightButton)
                    button = MouseButton.LeftButton;
            }
            return button;
        }

        private enum SystemMetric{
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }
    }
}