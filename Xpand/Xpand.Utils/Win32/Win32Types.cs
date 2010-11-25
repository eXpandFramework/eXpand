using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Xpand.Utils.Win32 {
    public class Win32Types {
        #region INPUTTYPE enum
        public enum INPUTTYPE {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }
        #endregion
        #region Nested type: DEVMODE
        public struct DEVMODE {
            /// <summary>
            /// Specifies the color resolution, in bits per pixel, of the display device (for example: 4 bits for 16 colors, 8 bits for 256 colors, or 16 bits for 65,536 colors). Display drivers use this member, for example, in the ChangeDisplaySettings function. Printer drivers do not use this member. 
            /// </summary>
            public short dmBitsPerPel;

            /// <summary>
            /// Used to align the structure to a DWORD boundary. This should not be used or referenced. Its name and usage is reserved, and can change in future releases. 
            /// </summary>
            public short dmCollate;

            /// <summary>
            /// Switches between color and monochrome on color printers. Following are the possible values
            /// </summary>
            public Win32Constants.DEVMODE_PrintQuality dmColor;

            /// <summary>
            /// Selects the number of copies printed if the device supports multiple-page copies. 
            /// </summary>
            public short dmCopies;

            /// <summary>
            /// Specifies the paper source. To retrieve a list of the available paper sources for a printer, use the DeviceCapabilities function with the DC_BINS flag. This member can be one of the following values, or it can be a device-specific value greater than or equal to DMBIN_USER
            /// </summary>
            public Win32Constants.DEVMODE_DefaultSource dmDefaultSource;

            /// <summary>
            /// Specifies the "friendly" name of the printer or display; for example, "PCL/HP LaserJet" in the case of PCL/HP LaserJet. This string is unique among device drivers. Note that this name may be truncated to fit in the dmDeviceName array. 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            /// <summary>
            /// Specifies the device's display mode. This member can be a combination of the following values. 
            /// </summary>
            public Win32Constants.DEVMODE_DisplayFlags dmDisplayFlags;

            /// <summary>
            /// Specifies the frequency, in hertz (cycles per second), of the display device in a particular mode. This value is also known as the display device's vertical refresh rate. Display drivers use this member. It is used, for example, in the ChangeDisplaySettings function. Printer drivers do not use this member. When you call the EnumDisplaySettings function, the dmDisplayFrequency member may return with the value 0 or 1. These values represent the display hardware's default refresh rate. This default rate is typically set by switches on a display card or computer motherboard, or by a configuration program that does not use display functions such as ChangeDisplaySettings. 
            /// </summary>
            public int dmDisplayFrequency;

            /// <summary>
            /// Windows 95/98/Me, Windows 2000/XP: Specifies how dithering is to be done. The member can be one of the following predefined values, or a driver-defined value greater than or equal to the value of DMDITHER_USER. 
            /// </summary>
            public Win32Constants.DEVMODE_DitherType dmDitherType;

            /// <summary>
            /// Contains the number of bytes of private driver-data that follow this structure. If a device driver does not use device-specific information, set this member to zero. 
            /// </summary>
            public short dmDriverExtra;

            /// <summary>
            /// Specifies the driver version number assigned by the driver developer. 
            /// </summary>
            public short dmDriverVersion;

            /// <summary>
            /// Selects duplex or double-sided printing for printers capable of duplex printing
            /// </summary>
            public Win32Constants.DEVMODE_Duplex dmDuplex;

            /// <summary>
            /// Specifies whether certain members of the DEVMODE structure have been initialized. If a member is initialized, its corresponding bit is set, otherwise the bit is clear. A driver supports only those DEVMODE members that are appropriate for the printer or display technology. 
            /// </summary>
            public Win32Constants.DEVMODE_dmfields dmFields;

            /// <summary>
            /// Windows NT/2000/XP: Specifies the name of the form to use; for example, "Letter" or "Legal". A complete set of names can be retrieved by using the EnumForms function. Windows 95/98/Me: Printer drivers do not use this member. 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            /// <summary>
            /// Windows 95/98/Me, Windows 2000/XP: Specifies which color matching method, or intent, should be used by default. This member is primarily for non-ICM applications. ICM applications can establish intents by using the ICM functions. This member can be one of the following predefined values, or a driver defined value greater than or equal to the value of DMICM_USER. 
            /// </summary>
            public Win32Constants.DEVMODE_ICMIntend dmICMIntent;

            /// <summary>
            /// Windows 95/98/Me; Windows 2000/XP: Specifies how ICM is handled. For a non-ICM application, this member determines if ICM is enabled or disabled. For ICM applications, the system examines this member to determine how to handle ICM support. This member can be one of the following predefined values, or a driver-defined value greater than or equal to the value of DMICMMETHOD_USER. The printer driver must provide a user interface for setting this member. Most printer drivers support only the DMICMMETHOD_SYSTEM or DMICMMETHOD_NONE value. Drivers for PostScript printers support all values. 
            /// </summary>
            public Win32Constants.DEVMODE_ICMMethods dmICMMethod;

            /// <summary>
            /// Specifies the number of pixels per logical inch. Printer drivers do not use this member. 
            /// </summary>
            public short dmLogPixels;

            /// <summary>
            /// Windows 95/98/Me, Windows 2000/XP: Specifies the type of media being printed on. The member can be one of the following predefined values, or a driver-defined value greater than or equal to the value of DMMEDIA_USER. 
            /// </summary>
            public Win32Constants.DEVMODE_MediaTypes dmMediaType;

            /// <summary>
            /// For printer devices only, selects the orientation of the paper. This member can be either DMORIENT_PORTRAIT (1) or DMORIENT_LANDSCAPE (2). 
            /// </summary>
            public short dmOrientation;

            /// <summary>
            /// Windows NT/2000/XP: This member must be zero. Windows 95/98/Me: This member is not supported. 
            /// </summary>
            public int dmPanningHeight;

            /// <summary>
            /// Windows NT/2000/XP: This member must be zero. Windows 95/98/Me: This member is not supported. 
            /// </summary>
            public int dmPanningWidth;

            /// <summary>
            /// For printer devices only, overrides the length of the paper specified by the dmPaperSize member, either for custom paper sizes or for devices such as dot-matrix printers that can print on a page of arbitrary length. These values, along with all other values in this structure that specify a physical length, are in tenths of a millimeter. 
            /// </summary>
            public short dmPaperLength;

            /// <summary>
            /// For printer devices only, selects the size of the paper to print on. This member can be set to zero if the length and width of the paper are both set by the dmPaperLength and dmPaperWidth members. Otherwise, the dmPaperSize member can be set to one of the following predefined values. 
            /// </summary>
            public short dmPaperSize;

            /// <summary>
            /// For printer devices only, overrides the width of the paper specified by the dmPaperSize member. 
            /// </summary>
            public short dmPaperWidth;

            /// <summary>
            /// Specifies the height, in pixels, of the visible device surface. Display drivers use this member, for example, in the ChangeDisplaySettings function. Printer drivers do not use this member. 
            /// </summary>
            public int dmPelsHeight;

            /// <summary>
            /// Specifies the width, in pixels, of the visible device surface. Display drivers use this member, for example, in the ChangeDisplaySettings function. Printer drivers do not use this member. 
            /// </summary>
            public int dmPelsWidth;

            /// <summary>
            /// Specifies the printer resolution. There are four predefined device-independent values
            /// </summary>
            public Win32Constants.DEVMODE_PrintQuality dmPrintQuality;

            public int dmReserved1;
            public int dmReserved2;

            /// <summary>
            /// Specifies the factor by which the printed output is to be scaled. The apparent page size is scaled from the physical page size by a factor of dmScale/100. For example, a letter-sized page with a dmScale value of 50 would contain as much data as a page of 17- by 22-inches because the output text and graphics would be half their original height and width. 
            /// </summary>
            public short dmScale;

            /// <summary>
            /// Specifies the size, in bytes, of the DEVMODE structure, not including any private driver-specific data that might follow the structure's public members. Set this member to sizeof(DEVMODE) to indicate the version of the DEVMODE structure being used. 
            /// </summary>
            public short dmSize;

            /// <summary>
            /// Specifies the version number of the initialization data specification on which the structure is based. To ensure the correct version is used for any operating system, use DM_SPECVERSION. 
            /// </summary>
            public short dmSpecVersion;

            /// <summary>
            /// Specifies how TrueType fonts should be printed. This member can be one of the following values. 
            /// </summary>
            public Win32Constants.DEVMODE_TTOptions dmTTOption;

            /// <summary>
            /// Specifies the y-resolution, in dots per inch, of the printer. If the printer initializes this member, the dmPrintQuality member specifies the x-resolution, in dots per inch, of the printer.
            /// </summary>
            public short dmYResolution;
        }
        #endregion
        #region Nested type: HARDWAREINPUT
        /// <summary>
        /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT {
            /// <summary>
            /// Value specifying the message generated by the input hardware. 
            /// </summary>
            public int uMsg;

            /// <summary>
            /// Specifies the low-order word of the lParam parameter for uMsg. 
            /// </summary>
            public short wParamL;

            /// <summary>
            /// Specifies the high-order word of the lParam parameter for uMsg. 
            /// </summary>
            public short wParamH;
        }
        #endregion
        #region Nested type: INPUT
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT {
            /// <summary>
            /// Indicates the type of device information this structure carries
            /// </summary>
            [FieldOffset(0)]
            public INPUTTYPE Type;

            /// <summary>
            /// MOUSEINPUT structure that contains information about simulated mouse input. 
            /// </summary>
            [FieldOffset(4)]
            public MOUSEINPUT mi;

            /// <summary>
            /// KEYBDINPUT structure that contains information about simulated keyboard input. 
            /// </summary>
            [FieldOffset(4)]
            public KEYBDINPUT ki;

            /// <summary>
            /// HARDWAREINPUT structure that contains information about a simulated input device message. 
            /// </summary>
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }
        #endregion
        #region Nested type: KEYBDINPUT
        /// <summary>
        /// The KEYBDINPUT structure contains information about a simulated keyboard event. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT {
            /// <summary>
            /// 'Specifies the size of the structure in bytes.  This is required by the cbSize Parameter of the SendInput Method.'The structure has 7 32 bit (4 byte) property values.
            /// </summary>
            public const int cbSize = 28;

            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
            /// </summary>
            public Win32Constants.VirtualKeys wVk;

            /// <summary>
            /// Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.
            /// </summary>
            public short wScan;

            /// <summary>
            /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values. 
            /// </summary>
            public Win32Constants.KeyboardEvent dwFlags;

            /// <summary>
            /// Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
            /// </summary>
            public int time;

            /// <summary>
            /// Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
            /// </summary>
            public IntPtr dwExtraInfo;

            //			public INPUTTYPE Type ;
        }
        #endregion
        #region Nested type: KeyboardHookStruct
        /// <summary>
        /// The KeyboardHookStruct structure contains information about a simulated keyboard event. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            public int vkCode;

            /// <summary>
            /// Specifies a hardware scan code for the key. 
            /// </summary>
            public int scanCode;

            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int flags;

            /// <summary>
            /// Specifies the time stamp for this message.
            /// </summary>
            public int time;

            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }
        #endregion
        #region Nested type: MENUITEMINFO
        [StructLayout(LayoutKind.Sequential)]
        public struct MENUITEMINFO {
            /// <summary>
            /// Size of structure, in bytes. The caller must set this to sizeof(MENUITEMINFO). 
            /// </summary>
            public int cbSize;

            /// <summary>
            /// Members to retrieve or set. This member can be one or more of these values. MIIM_BITMAP Microsoft® Windows® 98/Windows Millennium Edition (Windows Me), Windows 2000/Windows XP: Retrieves or sets the hbmpItem member.MIIM_CHECKMARKS Retrieves or sets the hbmpChecked and hbmpUnchecked members.MIIM_DATA Retrieves or sets the dwItemData member.MIIM_FTYPE Windows 98/Windows Me, Windows 2000/Windows XP: Retrieves or sets the fType member.MIIM_ID Retrieves or sets the wID member.MIIM_STATE Retrieves or sets the fState member. MIIM_STRING Windows 98/Windows Me, Windows 2000/Windows XP: Retrieves or sets the dwTypeData member.MIIM_SUBMENU Retrieves or sets the hSubMenu member.MIIM_TYPE Retrieves or sets the fType and dwTypeData members. Windows 98/Me, Windows 2000/XP: MIIM_TYPE is replaced by MIIM_BITMAP, MIIM_FTYPE, and MIIM_STRING.
            /// </summary>
            public int fMask;

            /// <summary>
            /// Menu item type. This member can be one or more of the following values.The MFT_BITMAP, MFT_SEPARATOR, and MFT_STRING values cannot be combined with one another. Set fMask to MIIM_TYPE to use fType.Windows 98/Me and Windows 2000/XP: fType is used only if fMask has a value of MIIM_FTYPE. MFT_BITMAP Displays the menu item using a bitmap. The low-order word of the dwTypeData member is the bitmap handle, and the cch member is ignored.Windows 98/Me, Windows 2000/XP: MFT_BITMAP is replaced by MIIM_BITMAP and hbmpItem. MFT_MENUBARBREAK Places the menu item on a new line (for a menu bar) or in a new column (for a drop-down menu, submenu, or shortcut menu). For a drop-down menu, submenu, or shortcut menu, a vertical line separates the new column from the old.MFT_MENUBREAK Places the menu item on a new line (for a menu bar) or in a new column (for a drop-down menu, submenu, or shortcut menu). For a drop-down menu, submenu, or shortcut menu, the columns are not separated by a vertical line.MFT_OWNERDRAW Assigns responsibility for drawing the menu item to the window that owns the menu. The window receives a WM_MEASUREITEM message before the menu is displayed for the first time, and a WM_DRAWITEM message whenever the appearance of the menu item must be updated. If this value is specified, the dwTypeData member contains an application-defined value.MFT_RADIOCHECK	Displays selected menu items using a radio-button mark instead of a check mark if the hbmpChecked member is NULL.MFT_RIGHTJUSTIFY Right-justifies the menu item and any subsequent items. This value is valid only if the menu item is in a menu bar.MFT_RIGHTORDER Windows 95/98/Me, Windows 2000/XP: Specifies that menus cascade right-to-left (the default is left-to-right). This is used 
            /// to support right-to-left languages, such as Arabic and Hebrew.MFT_SEPARATOR Specifies that the menu item is a separator. A menu item separator appears as a horizontal dividing line. The dwTypeData and cch members are ignored. This value is valid only in a drop-down menu, submenu, or 
            /// shortcut menu.MFT_STRING Displays the menu item using a text string. The dwTypeData member is the pointer to a null-terminated string, and the cch member is the length of the string.Windows 98/Me, Windows 2000/XP: MFT_STRING is replaced by MIIM_STRING.
            /// </summary>
            public int fType;

            /// <summary>
            /// Menu item state. This member can be one or more of these values. Set fMask to MIIM_STATE to use fState. MFS_CHECKED Checks the menu item. For more information about selected menu items, see the hbmpChecked member.MFS_DEFAULT Specifies that the menu item is the default. A menu can contain only one default menu item, which is displayed in bold.MFS_DISABLED Disables the menu item and grays it so that it cannot be selected. This is equivalent to MFS_GRAYED.MFS_ENABLED Enables the menu item so that it can be selected. This is the default state.MFS_GRAYED Disables the menu item and grays it so that it cannot be selected. This is equivalent to MFS_DISABLED.MFS_HILITE Highlights the menu item.MFS_UNCHECKED Unchecks the menu item. For more information about clear menu items, see the hbmpChecked member.MFS_UNHILITE Removes the highlight from the menu item. This is the default state.
            /// </summary>
            public int fState;

            /// <summary>
            /// Application-defined 16-bit value that identifies the menu item. Set fMask to MIIM_ID to use wID.
            /// </summary>
            public int wID;

            /// <summary>
            /// Handle to the drop-down menu or submenu associated with the menu item. If the menu item is not an item that opens a drop-down menu or submenu, this member is NULL. Set fMask to MIIM_SUBMENU to use hSubMenu.
            /// </summary>
            public int hSubMenu;

            /// <summary>
            /// Handle to the bitmap to display next to the item if it is selected. If this member is NULL, a default bitmap is used. If the MFT_RADIOCHECK type value is specified, the default bitmap is a bullet. Otherwise, it is a check mark. Set fMask to MIIM_CHECKMARKS to use hbmpChecked.
            /// </summary>
            public int hbmpChecked;

            /// <summary>
            /// Handle to the bitmap to display next to the item if it is not selected. If this member is NULL, no bitmap is used. Set fMask to MIIM_CHECKMARKS to use hbmpUnchecked
            /// </summary>
            public int hbmpUnchecked;

            /// <summary>
            /// Application-defined value associated with the menu item. Set fMask to MIIM_DATA to use dwItemData.
            /// </summary>
            public int dwItemData;

            /// <summary>
            /// Content of the menu item. The meaning of this member depends on the value of fType and is used only if the MIIM_TYPE flag is set in the fMask member. To retrieve a menu item of type MFT_STRING, first find the size of the string by setting the dwTypeData member of MENUITEMINFO to NULL and then calling GetMenuItemInfo. The value of cch+1 is the size needed. Then allocate a buffer of this size, place the pointer to the buffer in dwTypeData, increment cch, and call GetMenuItemInfo once again to fill the buffer with the string. If the retrieved menu item is of some other type, then GetMenuItemInfo sets the dwTypeData member to a value whose type is specified by the fType member. 
            /// </summary>
            public string dwTypeData;

            /// <summary>
            /// Length of the menu item text, in TCHARs, when information is received about a menu item of the MFT_STRING type. However, cch is used only if the MIIM_TYPE flag is set in the fMask member and is zero otherwise. Also, cch is ignored when the content of a menu item is set by calling SetMenuItemInfo. Note that, before calling GetMenuItemInfo, the application must set cch to the length of the buffer pointed to by the dwTypeData member. If the retrieved menu item is of type MFT_STRING (as indicated by the fType member), then GetMenuItemInfo changes cch to the length of the menu item text. If the retrieved menu item is of some other type, GetMenuItemInfo sets the cch field to zero. Windows 98/Me, Windows 2000/XP:  The cch member is used when the MIIM_STRING flag is set in the fMask member.
            /// </summary>
            public int cch;
        }
        #endregion
        #region Nested type: MOUSEINPUT
        /// <summary>
        /// The MOUSEINPUT structure contains information about a simulated mouse event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT {
            /// <summary>
            /// 'Specifies the size of the structure in bytes.  This is required by the cbSize Parameter of the SendInput Method.'The structure has 7 32 bit (4 byte) property values.
            /// </summary>
            public const int cbSize = 28;

            /// <summary>
            /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
            /// </summary>
            public int dx;

            /// <summary>
            /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
            /// </summary>
            public int dy;

            /// <summary>
            /// 
            /// </summary>
            public int mouseData;

            /// <summary>
            /// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
            /// </summary>
            public Win32Constants.MouseEvent dwFlags;

            /// <summary>
            /// Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
            /// </summary>
            public int time;

            /// <summary>
            /// Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
            /// </summary>
            public IntPtr dwExtraInfo;

            //			/// <summary>
            //			/// is used to specify the type of the input event
            //			/// </summary>
            //			public INPUTTYPE Type ;
            //
            //			public MOUSEINPUT(int mouseData, Win32Constants.MouseEvent dwFlags)
            //			{
            //				this.mouseData = mouseData;
            //				this.dwFlags = dwFlags;
            //				Type=INPUTTYPE.INPUT_MOUSE;
            //			}
        }
        #endregion
        #region Nested type: MouseHookStruct
        /// <summary>
        /// The MOUSEHOOKSTRUCT structure contains information about a mouse event passed to a WH_MOUSE hook procedure, MouseProc. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct {
            /// <summary>
            /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
            /// </summary>
            public POINT pt;

            /// <summary>
            /// Handle to the window that will receive the mouse message corresponding to the mouse event. 
            /// </summary>
            public int hwnd;

            /// <summary>
            /// Specifies the hit-test value. For a list of hit-test values, see the description of the WM_NCHITTEST message. 
            /// </summary>
            public int wHitTestCode;

            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }
        #endregion
        #region Nested type: POINT
        /// <summary>
        /// The Point structure defines the x- and y-coordinates of a point. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            #region Fields
            /// <summary>
            /// The x value of the point's coordinates.
            /// </summary>
            public int X;

            /// <summary>
            /// The y value of the point's coordinates.
            /// </summary>
            public int Y;
            #endregion
            #region Lifecycle
            /// <summary>
            /// Initializes a new instance of the <c>POINT</c> structure.
            /// </summary>
            public POINT(int x, int y) {
                X = x;
                Y = y;
            }
            #endregion
            #region Operator overloads
            /// <summary>Implicitly casts a <c>POINT</c> to a <see cref="Point"/>.</summary>
            /// <param name="p">The <c>POINT</c> instance to cast to a <c>Point</c> instance.</param>
            /// <returns>The casted <c>Point</c> structure.</returns>
            public static implicit operator Point(POINT p) {
                return new Point(p.X, p.Y);
            }

            /// <summary>Implicitly casts a <see cref="Point"/> to a <c>POINT</c>.</summary>
            /// <param name="p">The <c>Point</c> instance to cast to a <c>POINT</c> instance.</param>
            /// <returns>The casted <c>POINT</c> structure.</returns>
            public static implicit operator POINT(Point p) {
                return new POINT(p.X, p.Y);
            }
            #endregion
        }
        #endregion
        #region Nested type: RECT
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left_, int top_, int right_, int bottom_) {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

            public int Height {
                get { return Bottom - Top; }
            }

            public int Width {
                get { return Right - Left; }
            }

            public Size Size {
                get { return new Size(Width, Height); }
            }

            public Point Location {
                get { return new Point(Left, Top); }
            }

            /// <summary>
            /// Handy method for converting to a System.Drawing.Rectangle
            /// </summary>
            /// <returns></returns>
            public Rectangle ToRectangle() {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }

            public static RECT FromRectangle(Rectangle rectangle) {
                return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode() {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                       ^ ((Width << 0x1a) | (Width >> 6))
                       ^ ((Height << 7) | (Height >> 0x19));
            }
        }
        #endregion
        #region Nested type: WINDOWPLACEMENT
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT {
            /// <summary>
            /// Specifies the length, in bytes, of the structure. 
            /// </summary>
            public int length;

            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored
            /// </summary>
            public Win32Declares.Window.WINDOWPLACEMENTFLAGS flags;

            /// <summary>
            /// Specifies the current show state of the window
            /// </summary>
            public Win32Declares.Window.ShowWindowEnum showCmd;

            /// <summary>
            /// Specifies the position of the window's top-left corner when the window is minimized. 
            /// </summary>
            public POINT ptMinPosition;

            /// <summary>
            /// Specifies the position of the window's top-left corner when the window is maximized. 
            /// </summary>
            public POINT ptMaxPosition;

            /// <summary>
            /// Specifies the window's coordinates when the window is in the normal (restored) position. 
            /// </summary>
            public RECT rcNormalPosition;
        }
        #endregion
        #region Nested type: keybHookStruct
        public struct keybHookStruct {
            public int dwExtraInfo;
            public int flags;
            public int scanCode;
            public int time;
            public int vkCode;
        }
        #endregion
    }
}