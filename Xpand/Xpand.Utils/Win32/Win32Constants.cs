using System;

namespace Xpand.Utils.Win32 {
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public class Win32Constants {
        #region Button enum
        public enum Button {
            BM_GETCHECK = 0x00F0,
            BM_SETCHECK = 0x00F1,
            BM_GETSTATE = 0x00F2,
            BM_SETSTATE = 0x00F3,
            BM_SETSTYLE = 0x00F4,
            BM_CLICK = 0x00F5,
            BM_GETIMAGE = 0x00F6,
            BM_SETIMAGE = 0x00F7
        }
        #endregion
        #region ChildWindowFromPointFlags enum
        [Flags]
        public enum ChildWindowFromPointFlags {
            /// <summary>
            /// Does not skip any child windows
            /// </summary>
            CWP_ALL = 0,
            /// <summary>
            /// Skips invisible child windows
            /// </summary>
            CWP_SKIPINVISIBLE = 1,
            /// <summary>
            /// Skips disabled child windows
            /// </summary>
            CWP_SKIPDISABLED = 2,
            /// <summary>
            /// Skips transparent child windows
            /// </summary>
            CWP_SKIPTRANSPARENT = 4
        }
        #endregion
        #region Clipboard enum
        public enum Clipboard {
            WM_PASTE = 0x302
        }
        #endregion
        #region EnumDisplaySettings enum
        public enum EnumDisplaySettings {
            ENUM_CURRENT_SETTINGS = -1,
            ENUM_REGISTRY_SETTINGS = -2
        }
        #endregion
        #region
        #region DEVMODE_Color enum
        public enum DEVMODE_Color {
            DMCOLOR_MONOCHROME = 1,
            DMCOLOR_COLOR = 2
        }
        #endregion
        #region DEVMODE_DefaultSource enum
        public enum DEVMODE_DefaultSource : short {
            DMBIN_ONLYONE = 1,
            DMBIN_LOWER = 2,
            DMBIN_MIDDLE = 3,
            DMBIN_MANUAL = 4,
            DMBIN_ENVELOPE = 5,
            DMBIN_ENVMANUAL = 6,
            DMBIN_AUTO = 7,
            DMBIN_TRACTOR = 8,
            DMBIN_SMALLFMT = 9,
            DMBIN_LARGEFMT = 10,
            DMBIN_LARGECAPACITY = 11,
            DMBIN_CASSETTE = 14,
            DMBIN_FORMSOURCE = 15,
        }
        #endregion
        #region DEVMODE_DisplayFlags enum
        public enum DEVMODE_DisplayFlags {
            DM_GRAYSCALE = 0x00000001,
            DM_INTERLACED = 0x00000002
        }
        #endregion
        #region DEVMODE_DitherType enum
        public enum DEVMODE_DitherType {
            DMDITHER_NONE = 1,
            DMDITHER_COARSE = 2,
            DMDITHER_FINE = 3,
            DMDITHER_LINEART = 4,
            DMDITHER_ERRORDIFFUSION = 5,
            DMDITHER_GRAYSCALE = 10
        }
        #endregion
        #region DEVMODE_Duplex enum
        public enum DEVMODE_Duplex : short {
            DMDUP_SIMPLEX = 1,
            DMDUP_VERTICAL = 2,
            DMDUP_HORIZONTAL = 3
        }
        #endregion
        #region DEVMODE_ICMIntend enum
        public enum DEVMODE_ICMIntend {
            DMICM_SATURATE = 1,
            DMICM_CONTRAST = 2,
            DMICM_COLORIMETRIC = 3,
            DMICM_ABS_COLORIMETRIC = 4
        }
        #endregion
        #region DEVMODE_ICMMethods enum
        public enum DEVMODE_ICMMethods {
            DMICMMETHOD_NONE = 1,
            DMICMMETHOD_SYSTEM = 2,
            DMICMMETHOD_DRIVER = 3,
            DMICMMETHOD_DEVICE = 4
        }
        #endregion
        #region DEVMODE_MediaTypes enum
        public enum DEVMODE_MediaTypes {
            DMMEDIA_STANDARD = 1,
            DMMEDIA_TRANSPARENCY = 2,
            DMMEDIA_GLOSSY = 3
        }
        #endregion
        #region DEVMODE_PrintQuality enum
        public enum DEVMODE_PrintQuality : short {
            DMRES_DRAFT = -1,
            DMRES_LOW = -2,
            DMRES_MEDIUM = -3,
            DMRES_HIGH = -4,
        }
        #endregion
        #region DEVMODE_TTOptions enum
        public enum DEVMODE_TTOptions : short {
            DMTT_BITMAP = 1,
            DMTT_DOWNLOAD = 2,
            DMTT_SUBDEV = 3,
            DMTT_DOWNLOAD_OUTLINE = 4
        }
        #endregion
        #region DEVMODE_dmfields enum
        public enum DEVMODE_dmfields {
            DM_ORIENTATION = 0x00000001,
            DM_PAPERSIZE = 0x00000002,
            DM_PAPERLENGTH = 0x00000004,
            DM_PAPERWIDTH = 0x00000008,
            DM_SCALE = 0x00000010,
            DM_COPIES = 0x00000100,
            DM_DEFAULTSOURCE = 0x00000200,
            DM_PRINTQUALITY = 0x00000400,
            DM_COLOR = 0x00000800,
            DM_DUPLEX = 0x00001000,
            DM_YRESOLUTION = 0x00002000,
            DM_TTOPTION = 0x00004000,
            DM_COLLATE = 0x00008000,
            DM_FORMNAME = 0x00010000,
            DM_LOGPIXELS = 0x00020000,
            DM_BITSPERPEL = 0x00040000,
            DM_PELSWIDTH = 0x00080000,
            DM_PELSHEIGHT = 0x00100000,
            DM_DISPLAYFLAGS = 0x00200000,
            DM_DISPLAYFREQUENCY = 0x00400000,
            DM_ICMMETHOD = 0x00800000,
            DM_ICMINTENT = 0x01000000,
            DM_MEDIATYPE = 0x02000000,
            DM_DITHERTYPE = 0x04000000,
            DM_PANNINGWIDTH = 0x08000000,
            DM_PANNINGHEIGHT = 0x10000000,
        }
        #endregion
        #endregion
        #region ChangeDisplaySettings
        #region BroadCast enum
        public enum BroadCast {
            HWND_BROADCAST = 0xffff
        }
        #endregion
        #region BroadCastMessages enum
        public enum BroadCastMessages {
            WM_WININICHANGE = 0x001A
        }
        #endregion
        #region ChangeDisplaySettingsFlags enum
        [Flags]
        public enum ChangeDisplaySettingsFlags {
            /// <summary>
            /// The graphics mode for the current screen will be changed dynamically.
            /// </summary>
            None = 0x00000000,
            /// <summary>
            /// The graphics mode for the current screen will be changed dynamically and the graphics mode will be updated in the registry. The mode information is stored in the USER profile.
            /// </summary>
            CDS_UPDATEREGISTRY = 0x00000001,
            /// <summary>
            /// The system tests if the requested graphics mode could be set. 
            /// </summary>
            CDS_TEST = 0x00000002,
            /// <summary>
            /// The mode is temporary in nature. Windows NT/2000/XP: If you change to and from another desktop, this mode will not be reset. 
            /// </summary>
            CDS_FULLSCREEN = 0x00000004,
            /// <summary>
            /// The settings will be saved in the global settings area so that they will affect all users on the machine. Otherwise, only the settings for the user are modified. This flag is only valid when specified with the CDS_UPDATEREGISTRY flag.
            /// </summary>
            CDS_GLOBAL = 0x00000008,
            /// <summary>
            /// This device will become the primary device. 
            /// </summary>
            CDS_SET_PRIMARY = 0x00000010,

            CDS_VIDEOPARAMETERS = 0x00000020,
            /// <summary>
            /// The settings should be changed, even if the requested settings are the same as the current settings. 
            /// </summary>
            CDS_RESET = 0x40000000,
            /// <summary>
            /// The settings will be saved in the registry, but will not take affect. This flag is only valid when specified with the CDS_UPDATEREGISTRY flag. 
            /// </summary>
            CDS_NORESET = 0x10000000
        }
        #endregion
        #region ChangeDisplaySettingsResult enum
        public enum ChangeDisplaySettingsResult {
            /// <summary>
            /// The settings change was successful.
            /// </summary>
            DISP_CHANGE_SUCCESSFUL = 0,
            /// <summary>
            /// The computer must be restarted in order for the graphics mode to work.
            /// </summary>
            DISP_CHANGE_RESTART = 1,
            /// <summary>
            /// The display driver failed the specified graphics mode.
            /// </summary>
            DISP_CHANGE_FAILED = -1,
            /// <summary>
            /// The graphics mode is not supported.
            /// </summary>
            DISP_CHANGE_BADMODE = -2,
            /// <summary>
            /// Windows NT/2000/XP: Unable to write settings to the registry.
            /// </summary>
            DISP_CHANGE_NOTUPDATED = -3,
            /// <summary>
            /// An invalid set of flags was passed in.
            /// </summary>
            DISP_CHANGE_BADFLAGS = -4,
            /// <summary>
            /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
            /// </summary>
            DISP_CHANGE_BADPARAM = -5,
            /// <summary>
            /// Windows XP: The settings change was unsuccessful because system is DualView capable.
            /// </summary>
            DISP_CHANGE_BADDUALVIEW = -6
        }
        #endregion
        #endregion
        #region Focus enum
        public enum Focus {
            WM_SETFOCUS = 7,
            WM_KILLFOCUS = 0x0008
        }
        #endregion
        #region HWNDEnum enum
        public enum HWNDEnum {
            HWND_TOP = 0,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }
        #endregion
        #region HookContants enum
        public enum HookContants {
            /// <summary>
            /// mouse hook constant
            /// </summary>
            WH_MOUSE_LL = 14,
            /// <summary>
            /// keyboard hook constant	
            /// </summary>
            WH_KEYBOARD_LL = 13
        }
        #endregion
        #region KeyBoard enum
        public enum KeyBoard {
            wpKeyDown = 256,
            wpKeyUp = 257,
            KEYISDOWN = 0x8000,
            /// <summary>
            /// to be used with as keybd_event dwFlag. Denotes that a key has been released
            /// </summary>
            KEYEVENTF_KEYUP = 0x2,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_CHAR = 0x102
        }
        #endregion
        #region KeyboardEvent enum

        [Flags]
        public enum KeyboardEvent {
            KEYEVENTF_EXTENDEDKEY = 0x1,
            KEYEVENTF_KEYUP = 0x2,
            KEYEVENTF_UNICODE = 0x4,
            KEYEVENTF_SCANCODE = 0x8,
        }
        #endregion
        #region ListBox enum
        public enum ListBox {
            LB_ADDSTRING = 0x0180,
            LB_INSERTSTRING = 0x0181,
            LB_DELETESTRING = 0x0182,
            LB_SELITEMRANGEEX = 0x0183,
            LB_RESETCONTENT = 0x0184,
            LB_SETSEL = 0x0185,
            LB_SETCURSEL = 0x0186,
            LB_GETSEL = 0x0187,
            LB_GETCURSEL = 0x0188,
            LB_GETTEXT = 0x0189,
            LB_GETTEXTLEN = 0x018A,
            LB_GETCOUNT = 0x018B,
            LB_SELECTSTRING = 0x018C,
            LB_DIR = 0x018D,
            LB_GETTOPINDEX = 0x018E,
            LB_FINDSTRING = 0x018F,
            LB_GETSELCOUNT = 0x0190,
            LB_GETSELITEMS = 0x0191,
            LB_SETTABSTOPS = 0x0192,
            LB_GETHORIZONTALEXTENT = 0x0193,
            LB_SETHORIZONTALEXTENT = 0x0194,
            LB_SETCOLUMNWIDTH = 0x0195,
            LB_ADDFILE = 0x0196,
            LB_SETTOPINDEX = 0x0197,
            LB_GETITEMRECT = 0x0198,
            LB_GETITEMDATA = 0x0199,
            LB_SETITEMDATA = 0x019A,
            LB_SELITEMRANGE = 0x019B,
            LB_SETANCHORINDEX = 0x019C,
            LB_GETANCHORINDEX = 0x019D,
            LB_SETCARETINDEX = 0x019E,
            LB_GETCARETINDEX = 0x019F,
            LB_SETITEMHEIGHT = 0x01A0,
            LB_GETITEMHEIGHT = 0x01A1,
            LB_FINDSTRINGEXACT = 0x01A2,
            LB_SETLOCALE = 0x01A5,
            LB_GETLOCALE = 0x01A6,
            LB_SETCOUNT = 0x01A7
        }
        #endregion
        #region Mouse enum
        public enum Mouse {
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_RBUTTONDOWN = 0x204,
            WM_MBUTTONDOWN = 0x207,
            WM_LBUTTONUP = 0x202,
            WM_RBUTTONUP = 0x205,
            WM_MBUTTONUP = 0x208,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDBLCLK = 0x209,
        }
        #endregion
        #region MouseEvent enum

        [Flags]
        public enum MouseEvent {
            MOUSEEVENTF_MOVE = 0x00000001,
            MOUSEEVENTF_LEFTDOWN = 0x00000002,
            MOUSEEVENTF_LEFTUP = 0x00000004,
            MOUSEEVENTF_RIGHTDOWN = 0x00000008,
            MOUSEEVENTF_RIGHTUP = 0x00000010,
            MOUSEEVENTF_MIDDLEDOWN = 0x00000020,
            MOUSEEVENTF_MIDDLEUP = 0x00000040,
            MOUSEEVENTF_VWHEEL = 0x00000800,
            MOUSEEVENTF_ABSOLUTE = 0x00008000,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_HWHEEL = 0x1000
        }
        #endregion
        #region SendMessageTimeoutFlags enum
        [Flags]
        public enum SendMessageTimeoutFlags : uint {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }
        #endregion
        #region SetWindowPosEnum enum
        public enum SetWindowPosEnum : uint {
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues; the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. 
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,
            /// <summary>
            /// Prevents generation of the WM_SYNCPAINT message. 
            /// </summary>
            SWP_DEFERERASE = 0x2000,
            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = SWP_FRAMECHANGED,
            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window; even if the window's size is not being changed. If this flag is not specified; WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x20,
            /// <summary>
            /// Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x80,
            /// <summary>
            /// Does not activate the window. If this flag is not set; the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x10,
            /// <summary>
            /// Discards the entire contents of the client area. If this flag is not specified; the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x100,
            /// <summary>
            /// Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x2,
            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x200,
            /// <summary>
            /// Does not redraw changes. If this flag is set; no repainting of any kind occurs. This applies to the client area; the nonclient area (including the title bar and scroll bars); and any part of the parent window uncovered as a result of the window being moved. When this flag is set; the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x8,
            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = SWP_NOOWNERZORDER,
            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x400,
            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x1,
            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x4,
            /// <summary>
            /// Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x40,
        }
        #endregion
        #region Standard enum
        public enum Standard {
            WM_SYSCOMMAND = 0x0112,
            SC_CLOSE = 0xF060,
            WM_COMMAND = 0x111,
            WM_SETTEXT = 0xC,
            WM_GETTEXT = 0xD,
            WM_GETTEXTLENGTH = 0x000E
        }
        #endregion
        #region TernaryRasterOperations enum
        public enum TernaryRasterOperations {
            /// <summary>
            /// /* dest = source*/
            /// </summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>
            /// /* dest = source OR dest*/
            /// </summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>
            /// /* dest = source AND dest*/
            /// </summary>
            SRCAND = 0x008800C6,
            /// <summary>
            /// /* dest = source XOR dest*/
            /// </summary>
            SRCINVERT = 0x00660046,
            /// <summary>
            /// /* dest = source AND (NOT dest )*/
            /// </summary>
            SRCERASE = 0x00440328,
            /// <summary>
            /// /* dest = (NOT source)*/
            /// </summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>
            /// /* dest = (NOT src) AND (NOT dest) */
            /// </summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>
            /// /* dest = (source AND pattern)*/
            /// </summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>
            /// /* dest = (NOT source) OR dest*/
            /// </summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>
            /// /* dest = pattern*/
            /// </summary>
            PATCOPY = 0x00F00021,
            /// <summary>
            /// /* dest = DPSnoo*/
            /// </summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>
            /// /* dest = pattern XOR dest*/
            /// </summary>
            PATINVERT = 0x005A0049,
            /// <summary>
            /// /* dest = (NOT dest)*/
            /// </summary>
            DSTINVERT = 0x00550009,
            /// <summary>
            /// /* dest = BLACK*/
            /// </summary>
            BLACKNESS = 0x00000042,
            /// <summary>
            /// /* dest = WHITE*/
            /// </summary>
            WHITENESS = 0x00FF0062,
        }
        #endregion
        #region VirtualKeys enum
        public enum VirtualKeys : short {
            Back = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,

            ShiftLeft = 0xA0,
            ControlLeft = 0xA2,
            ShiftRight = 0xA1,
            ControlRight = 0xA3,
            AltLeft = 0xA4,
            AltRight = 0xA5,

            Menu = 0x12,
            Pause = 0x13,
            Capital = 0x14,
            Escape = 0x1B,
            Space = 0x20,
            Prior = 0x21,
            Next = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            Snapshot = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,

            D0 = 0x30,
            D1 = 0x31,
            D2 = 0x32,
            D3 = 0x33,
            D4 = 0x34,
            D5 = 0x35,
            D6 = 0x36,
            D7 = 0x37,
            D8 = 0x38,
            D9 = 0x39,

            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,

            LWindows = 0x5B,
            RWindows = 0x5C,
            Apps = 0x5D,
            NumPad0 = 0x60,
            NumPad1 = 0x61,
            NumPad2 = 0x62,
            NumPad3 = 0x63,
            NumPad4 = 0x64,
            NumPad5 = 0x65,
            NumPad6 = 0x66,
            NumPad7 = 0x67,
            NumPad8 = 0x68,
            NumPad9 = 0x69,

            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,

            NumLock = 0x90,
            Scroll = 0x91,
            LMenu=0xA4,
            RMenu=0xA5,
            Control=0x11,
            Cancel=0x03
        }
        #endregion
    }
}