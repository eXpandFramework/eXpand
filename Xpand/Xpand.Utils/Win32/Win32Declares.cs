using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Xpand.Utils.Win32 {
    /// <summary>
    /// Summary description for Win32Declares.
    /// </summary>
    public class Win32Declares {
        public class COM {
            /// <summary>
            /// Supplies a pointer to the IRunningObjectTable interface on the local Running Object Table (ROT).
            /// </summary>
            /// <param name="reserved">[in] Reserved for future use; must be zero. </param>
            /// <param name="pprot">[out] Address of IRunningObjectTable* pointer variable that receives the interface pointer to the local ROT. When the function is successful, the caller is responsible for calling IUnknown::Release on the interface pointer. If an error occurs, *pprot is undefined</param>
            /// <returns>This function supports the standard return value E_UNEXPECTED, as well as the following: S_OK An IRunningObjectTable pointer was successfully returned.</returns>
            /// <remarks>Each workstation has a local ROT that maintains a table of the objects that have been registered as running on that machine. This function returns an IRunningObjectTable interface pointer, which provides access to that table. Moniker providers, which hand out monikers that identify objects so they are accessible to others, should call GetRunningObjectTable. Use the interface pointer returned by this function to register your objects when they begin running, to record the times that those objects are modified, and to revoke their registrations when they stop running. See the IRunningObjectTable interface for more information.Compound-document link sources are the most common example of moniker providers. These include server applications that support linking to their documents (or portions of a document) and container applications that support linking to embeddings within their documents. Server applications that do not support linking can also use the ROT to cooperate with container applications that support linking to embeddings. If you are implementing the IMoniker interface to write a new moniker class, and you need an interface pointer to the ROT, call IBindCtx::GetRunningObjectTable rather than the GetRunningObjectTable function. This allows future implementations of the IBindCtx interface to modify binding behavior</remarks>
            [DllImport("ole32.dll")]
#pragma warning disable 612,618
            public static extern int GetRunningObjectTable(uint reserved, out UCOMIRunningObjectTable pprot);
#pragma warning restore 612,618

            /// <summary>
            /// Supplies a pointer to an implementation of IBindCtx (a bind context object). This object stores information about a particular moniker-binding operation. The pointer this function supplies is required as a parameter in many methods of the IMoniker interface and in certain functions related to monikers
            /// </summary>
            /// <param name="reserved">[in] Reserved for future use; must be zero. </param>
            /// <param name="ppbc">[out] Address of IBindCtx* pointer variable that receives the interface pointer to the new bind context object. When the function is successful, the caller is responsible for calling IUnknown::Release on the bind context. A NULL value for the bind context indicates that an error occurred. </param>
            /// <returns>This function supports the standard return value E_OUTOFMEMORY, as well as the following: S_OK The bind context was allocated and initialized successfully.</returns>
            [DllImport("ole32.dll")]
#pragma warning disable 612,618
            public static extern int CreateBindCtx(int reserved, out UCOMIBindCtx ppbc);
#pragma warning restore 612,618

        }

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard state to the corresponding character or characters. The function translates the code using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">[in] Specifies the virtual-key code to be translated. </param>
        /// <param name="uScanCode">[in] Specifies the hardware scan code of the key to be translated. The high-order bit of this value is set if the key is up (not pressed). </param>
        /// <param name="lpbKeyState">[in] Pointer to a 256-byte array that contains the current keyboard state. Each element (byte) in the array contains the state of one key. If the high-order bit of a byte is set, the key is down (pressed). The low bit, if set, indicates that the key is toggled on. In this function, only the toggle bit of the CAPS LOCK key is relevant. The toggle state of the NUM LOCK and SCROLL LOCK keys is ignored.</param>
        /// <param name="lpwTransKey">[out] Pointer to the buffer that receives the translated character or characters. </param>
        /// <param name="fuState">[in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. </param>
        /// <returns>If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
        /// <para>The specified virtual key has no translation for the current state of the keyboard. </para>
        /// <para>One character was copied to the buffer. </para>
        /// <para>Two characters were copied to the buffer. This usually happens when a dead-key character (accent or diacritic) stored in the keyboard layout cannot be composed with the specified virtual key to form a single character.</para>  
        /// </returns>
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        public class Menu {
            /// <summary>
            /// The GetMenuItemID function retrieves the menu item identifier of a menu item located at the specified position in a menu. 
            /// </summary>
            /// <param name="hMenu">[in] Handle to the menu that contains the item whose identifier is to be retrieved</param>
            /// <param name="nPos">[in] Specifies the zero-based relative position of the menu item whose identifier is to be retrieved</param>
            /// <returns>The return value is the identifier of the specified menu item. If the menu item identifier is NULL or if the specified item opens a submenu, the return value is -1. </returns>
            [DllImport("user32.dll")]
            public static extern int GetMenuItemID(IntPtr hMenu, int nPos);

            /// <summary>
            /// The GetMenu function retrieves a handle to the menu assigned to the specified window. 
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window whose menu handle is to be retrieved</param>
            /// <returns>The return value is a handle to the menu. If the specified window has no menu, the return value is NULL. If the window is a child window, the return value is undefined</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetMenu(IntPtr hwnd);

            /// <summary>
            /// The GetSubMenu function retrieves a handle to the drop-down menu or submenu activated by the specified menu item. 
            /// </summary>
            /// <param name="hMenu">[in] Handle to the menu. </param>
            /// <param name="nPos">[in] Specifies the zero-based relative position in the specified menu of an item that activates a drop-down menu or submenu.</param>
            /// <returns>If the function succeeds, the return value is a handle to the drop-down menu or submenu activated by the menu item. If the menu item does not activate a drop-down menu or submenu, the return value is NULL. </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);
        }

        public class Message {
            public const int EM_SETSEL = 0x00B1;
            #region PostMessage
            /// <summary>
            /// The PostMessage function places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message. 
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window whose window procedure is to receive the message. The following values have special meanings.HWND_BROADCAST The message is posted to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows. The message is not posted to child windows. NULL The function behaves like a call to PostThreadMessage with the dwThreadId parameter set to the identifier of the current thread.</param>
            /// <param name="wMsg">[in] Specifies the message to be posted.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            public static extern int PostMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);

            /// <summary>
            /// The PostMessage function places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message. 
            /// </summary>
            /// <param name="handle">[in] Handle to the window whose window procedure is to receive the message. The following values have special meanings.HWND_BROADCAST The message is posted to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows. The message is not posted to child windows. NULL The function behaves like a call to PostThreadMessage with the dwThreadId parameter set to the identifier of the current thread.</param>
            /// <param name="keydown">[in] Specifies the message to be posted.</param>
            /// <param name="keys">[in] Specifies additional message-specific information</param>
            /// <param name="intPtr">[in] Specifies additional message-specific information.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            public static extern void PostMessage(IntPtr handle, Win32Constants.KeyBoard keydown, Win32Constants.VirtualKeys keys, IntPtr intPtr);

            /// <summary>
            /// The PostMessage function places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message. 
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window whose window procedure is to receive the message. The following values have special meanings.HWND_BROADCAST The message is posted to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows. The message is not posted to child windows. NULL The function behaves like a call to PostThreadMessage with the dwThreadId parameter set to the identifier of the current thread.</param>
            /// <param name="wMsg">[in] Specifies the message to be posted.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            public static extern int PostMessage(IntPtr hwnd, Win32Constants.Standard wMsg, int wParam, int lParam);

            /// <summary>
            /// The PostMessage function places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message. 
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window whose window procedure is to receive the message. The following values have special meanings.HWND_BROADCAST The message is posted to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows. The message is not posted to child windows. NULL The function behaves like a call to PostThreadMessage with the dwThreadId parameter set to the identifier of the current thread.</param>
            /// <param name="wMsg">[in] Specifies the message to be posted.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            public static extern int PostMessage(IntPtr hwnd, Win32Constants.Button wMsg, IntPtr wParam, IntPtr lParam);
            #endregion
            #region SendMessage
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessageTimeout(IntPtr hWnd, Win32Constants.Standard msg, Win32Constants.Standard wParam, int lParam, Win32Constants.SendMessageTimeoutFlags fuFlags, uint uTimeout, out int lpdwResult);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="s">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.Standard msg, IntPtr wParam, string s);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.Standard msg, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="stringBuilder">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.Standard msg, IntPtr wParam, [Out] StringBuilder stringBuilder);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="stringBuilder">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.ListBox msg, IntPtr wParam, [Out] StringBuilder stringBuilder);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="s">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.ListBox msg, IntPtr wParam, string s);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.ListBox msg, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.Button msg, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.Focus msg, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message. To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="msg">[in] Specifies the message to be sent.</param>
            /// <param name="wParam">[in] Specifies additional message-specific information.</param>
            /// <param name="lParam">[in] Specifies additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Win32Constants.Clipboard msg, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// The SendNotifyMessage function sends the specified message to a window or windows. If the window was created by the calling thread, SendNotifyMessage calls the window procedure for the window and does not return until the window procedure has processed the message. If the window was created by a different thread, SendNotifyMessage passes the message to the window procedure and returns immediately; it does not wait for the window procedure to finish processing the message.
            /// </summary>
            /// <param name="broadCast">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="broadCastMessages"></param>
            /// <param name="wParam"></param>
            /// <param name="lParam">use "windows"</param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SendNotifyMessage(Win32Constants.BroadCast broadCast, Win32Constants.BroadCastMessages broadCastMessages, UIntPtr wParam,
                                                        StringBuilder lParam);
            #endregion
            /// <summary>
            /// The SendNotifyMessage function sends the specified message to a window or windows. If the window was created by the calling thread, SendNotifyMessage calls the window procedure for the window and does not return until the window procedure has processed the message. If the window was created by a different thread, SendNotifyMessage passes the message to the window procedure and returns immediately; it does not wait for the window procedure to finish processing the message.
            /// </summary>
            /// <param name="ptr">[in] Handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
            /// <param name="keyState"></param>
            /// <param name="virtualKeys"></param>
            /// <param name="intPtr">use "windows"</param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern void SendMessage(IntPtr ptr, Win32Constants.KeyBoard keyState, Win32Constants.VirtualKeys virtualKeys, IntPtr intPtr);


        }

        public class KeyBoard {
            [DllImport("user32.dll")]
            public static extern bool DestroyCaret();
            [DllImport("user32.dll", SetLastError=true)]
            public static extern bool HideCaret(IntPtr hWnd);
            /// <summary>
            /// The SendInput function synthesizes keystrokes, mouse motions, and button clicks.
            /// </summary>
            /// <param name="nInputs">[in] Specifies the number of structures in the pInputs array. </param>
            /// <param name="pInputs">[in] Pointer to an array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream. </param>
            /// <param name="cbSize">[in] Specifies the size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function will fail. </param>
            /// <returns>The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread.</returns>
            [DllImport("user32.dll")]
            public static extern uint SendInput(uint nInputs, Win32Types.INPUT[] pInputs, int cbSize);

            /// <summary>
            /// The keybd_event function synthesizes a keystroke. The system can use such a synthesized keystroke to generate a WM_KEYUP or WM_KEYDOWN message. The keyboard driver's interrupt handler calls the keybd_event function.
            /// </summary>
            /// <param name="bVk">[in] Specifies a virtual-key code. The code must be a value in the range 1 to 254. For a complete list, see Virtual-Key Codes. </param>
            /// <param name="bScan">This parameter is not used.</param>
            /// <param name="dwFlags">[in] Specifies various aspects of function operation. This parameter can be one or more of the following values. 
            /// <para>KEYEVENTF_EXTENDEDKEY If specified, the scan code was preceded by a prefix byte having the value 0xE0 (224).</para>
            /// <para>KEYEVENTF_KEYUP If specified, the key is being released. If not specified, the key is being depressed.</para>
            /// </param>
            /// <param name="dwExtraInfo">[in] Specifies an additional value associated with the key stroke. </param>
            [DllImport("user32.dll")]
            public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            /// <summary>
            /// The GetKeyboardState function copies the status of the 256 virtual keys to the specified buffer. 
            /// </summary>
            /// <param name="pbKeyState">[in] Pointer to the 256-byte array that receives the status data for each virtual key.</param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
            [DllImport("user32")]
            public static extern int GetKeyboardState(byte[] pbKeyState);

            /// <summary>
            /// The GetAsyncKeyState function determines whether a key is up or down at the time the function is called, and whether the key was pressed after a previous call to GetAsyncKeyState. 
            /// </summary>
            /// <param name="vKey">[in] Specifies one of 256 possible virtual-key codes.Windows NT: You can use left- and right-distinguishing constants to specify certain keys. See the Remarks section for further information. Windows 95: Windows 95 does not support the left- and right-distinguishing constants available on Windows NT. </param>
            /// <returns>If the function succeeds, the return value specifies whether the key was pressed since the last call to GetAsyncKeyState, and whether the key is currently up or down. If the most significant bit is set, the key is down, and if the least significant bit is set, the key was pressed after the previous call to GetAsyncKeyState. The return value is zero if a window in another thread or process currently has the keyboard focus. Windows 95: Windows 95 does not support the left- and right-distinguishing constants. If you call GetAsyncKeyState on the Windows 95 platform with these constants, the return value is zero. </returns>
            [DllImport("user32.dll")]
            public static extern int GetAsyncKeyState(int vKey);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
            public static extern short GetKeyState(int keyCode);
        }

        public class Hooks {
            public delegate int KeyBoardCatcherDelegate(int code, int wparam, ref Win32Types.keybHookStruct lparam);

            public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

            [DllImport("user32", EntryPoint = "SetWindowsHookExA")]
            public static extern int SetWindowsHookEx(int idHook, KeyBoardCatcherDelegate lpfn, int hmod, int dwThreadId);


            [DllImport("user32", EntryPoint = "UnhookWindowsHookEx")]
            public static extern int UnhookWindowsHookEx(int hHook);

            //		/// <summary>
            //		/// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
            //		/// </summary>
            //		/// <param name="idHook">Identifies the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx.</param>
            //		/// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
            //		[DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
            //		public static extern bool UnhookWindowsHookEx(int idHook);

            /// <summary>
            /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information.
            /// </summary>
            /// <param name="idHook">Identifies the current hook. An application receives this handle as a result of a previous call to the SetWindowsHookEx function.</param>
            /// <param name="nCode">Specifies the hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
            /// <param name="wParam">Specifies the wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <param name="lParam">Specifies the lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <returns>If the function succeeds, the return value is the value returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures. </returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

            [DllImport("user32", EntryPoint = "CallNextHookEx")]
            public static extern int CallNextKeyboardHook(int hHook, int ncode, int wParam, Win32Types.keybHookStruct lParam);
        }

        public class Window{
            public enum ShowScrollBarEnum{
                SB_HORZ = 0,
                SB_VERT = 1,
                SB_CTL = 2,
                SB_BOTH = 3
            }
            
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ShowScrollBar(IntPtr hWnd, ShowScrollBarEnum wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);
            /// <summary>
            /// The IsIconic function determines whether the specified window is minimized (iconic). 
            /// </summary>
            /// <param name="hWnd"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool IsIconic(IntPtr hWnd);

            /// <summary>
            /// The DestroyWindow function destroys the specified window. The function sends WM_DESTROY and WM_NCDESTROY messages to the window to deactivate it and remove the keyboard focus from it. The function also destroys the window's menu, flushes the thread message queue, destroys timers, removes clipboard ownership, and breaks the clipboard viewer chain (if the window is at the top of the viewer chain).If the specified window is a parent or owner window, DestroyWindow automatically destroys the associated child or owned windows when it destroys the parent or owner window. The function first destroys child or owned windows, and then it destroys the parent or owner window.DestroyWindow also destroys modeless dialog boxes created by the CreateDialog function
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window to be destroyed. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool DestroyWindow(IntPtr hWnd);

            /// <summary>
            /// The IsWindowEnabled function determines whether the specified window is enabled for mouse and keyboard input. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window to test. </param>
            /// <returns>If the window is enabled, the return value is nonzero.If the window is not enabled, the return value is zero.</returns>
            [DllImport("user32.dll")]
            public static extern bool IsWindowEnabled(IntPtr hWnd);

            /// <summary>
            /// The EnableWindow function enables or disables mouse and keyboard input to the specified window or control. When input is disabled, the window does not receive input such as mouse clicks and key presses. When input is enabled, the window receives all input. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window to be enabled or disabled. </param>
            /// <param name="bEnable">[in] Specifies whether to enable or disable the window. If this parameter is TRUE, the window is enabled. If the parameter is FALSE, the window is disabled. </param>
            /// <returns>If the window was previously disabled, the return value is nonzero.If the window was not previously disabled, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

            /// <summary>
            /// The SetWindowText function changes the text of the specified window's title bar (if it has one). If the specified window is a control, the text of the control is changed. However, SetWindowText cannot change the text of a control in another application.
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window or control whose text is to be changed.</param>
            /// <param name="stringBuilder">[in] Pointer to a null-terminated string to be used as the new title or control text. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool SetWindowText(IntPtr hWnd, StringBuilder stringBuilder);

            /// <summary>
            /// The ChildWindowFromPointEx function determines which, if any, of the child windows belonging to the specified parent window contains the specified point. The function can ignore invisible, disabled, and transparent child windows. The search is restricted to immediate child windows, grandchildren and deeper descendants are not searched. 
            /// </summary>
            /// <param name="hWndParent">[in] Handle to the parent window. </param>
            /// <param name="pt">[in] Specifies a POINT structure that defines the client coordinates (relative to hwndParent) of the point to be checked. </param>
            /// <param name="childWindowFromPointFlags">[in] Specifies which child windows to skip</param>
            /// <returns>The return value is a handle to the first child window that contains the point and meets the criteria specified by uFlags. If the point is within the parent window but not within any child window that meets the criteria, the return value is a handle to the parent window. If the point lies outside the parent window or if the function fails, the return value is NULL.</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, Win32Types.POINT pt, Win32Constants.ChildWindowFromPointFlags childWindowFromPointFlags);

            /// <summary>
            /// The WindowFromPoint function retrieves a handle to the window that contains the specified point. 
            /// </summary>
            /// <param name="point">[in] Specifies a POINT structure that defines the point to be checked. </param>
            /// <returns>The return value is a handle to the window that contains the point. If no window exists at the given point, the return value is NULL. If the point is over a static text control, the return value is a handle to the window under the static text control. </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr WindowFromPoint(Win32Types.POINT point);

            /// <summary>
            /// The MoveWindow function changes the position and dimensions of the specified window. For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. For a child window, they are relative to the upper-left corner of the parent window's client area. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window. </param>
            /// <param name="x">[in] Specifies the new position of the left side of the window. </param>
            /// <param name="y">[in] Specifies the new position of the top of the window. </param>
            /// <param name="nWidth">[in] Specifies the new width of the window. </param>
            /// <param name="nHeight">[in] Specifies the new height of the window. </param>
            /// <param name="bRepaint">[in] Specifies whether the window is to be repainted. If this parameter is TRUE, the window receives a message. If the parameter is FALSE, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of moving a child window. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
            #region Get/SET WindowPlacement
            public enum WINDOWPLACEMENTFLAGS {
                WPF_SETMINPOSITION = 0x1,
                WPF_RESTORETOMAXIMIZED = 0x2

            }


            /// <summary>
            /// The SetWindowPlacement function sets the show state and the restored, minimized, and maximized positions of the specified window. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window. </param>
            /// <param name="lpwndpl">[in] Pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if lpwndpl->length is not set correctly.</param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref Win32Types.WINDOWPLACEMENT lpwndpl);

            /// <summary>
            /// The GetWindowPlacement function retrieves the show state and the restored, minimized, and maximized positions of the specified window. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window. </param>
            /// <param name="lpwndpl">[out] Pointer to the WINDOWPLACEMENT structure that receives the show state and position information. Before calling GetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool GetWindowPlacement(IntPtr hWnd, out Win32Types.WINDOWPLACEMENT lpwndpl);
            #endregion
            /// <summary>
            /// The GetWindowTextLength function retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar). If the specified window is a control, the function retrieves the length of the text within the control. However, GetWindowTextLength cannot retrieve the length of the text of an edit control in another application.
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window or control. </param>
            /// <returns>If the function succeeds, the return value is the length, in characters, of the text. Under certain conditions, this value may actually be greater than the length of the text. For more information, see the following Remarks section.If the window has no text, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern int GetWindowTextLength(IntPtr hwnd);

            /// <summary>
            /// The GetWindowText function copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another application.
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window or control containing the text. </param>
            /// <param name="stringBuilder">[out] Pointer to the buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a NULL character. </param>
            /// <param name="cch">[in] Specifies the maximum number of characters to copy to the buffer, including the NULL character. If the text exceeds this limit, it is truncated</param>
            /// <returns>If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating NULL character. If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero. To get extended error information, call GetLastError. This function cannot retrieve the text of an edit control in another application.</returns>
            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hwnd, StringBuilder stringBuilder, int cch);

            public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

            /// <summary>
            /// The EnumChildWindows function enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function. EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE. 
            /// </summary>
            /// <param name="hwnd">[in] Handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows. Windows 95/98/Me: hWndParent cannot be NULL. 
            ///</param>
            /// <param name="func">[in] Pointer to an application-defined callback function. For more information, see EnumChildProc. </param>
            /// <param name="lParam">[in] Specifies an application-defined value to be passed to the callback function. </param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
            #region SetWindowPos
            /// <summary>
            /// The SetWindowPos function changes the size, position, and Z order of a child, pop-up, or top-level window. Child, pop-up, and top-level windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window.</param>
            /// <param name="hWndInsertAfter">[in] Handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values.HWND_BOTTOM Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.HWND_NOTOPMOST Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.HWND_TOP Places the window at the top of the Z order.HWND_TOPMOST Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.For more information about how this parameter is used, see the following Remarks section</param>
            /// <param name="x">[in] Specifies the new position of the left side of the window, in client coordinates. </param>
            /// <param name="y">[in] Specifies the new position of the top of the window, in client coordinates. </param>
            /// <param name="cx">[in] Specifies the new width of the window, in pixels. </param>
            /// <param name="cy">[in] Specifies the new height of the window, in pixels.</param>
            /// <param name="setWindowPosEnum">[in] Specifies the window sizing and positioning flags. This parameter can be a combination of the following values. SWP_ASYNCWINDOWPOS If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. SWP_DEFERERASE Prevents generation of the WM_SYNCPAINT message. SWP_DRAWFRAME Draws a frame (defined in the window's class description) around the window.SWP_FRAMECHANGED Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// SWP_HIDEWINDOW Hides the window.SWP_NOACTIVATE Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).SWP_NOCOPYBITS Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.SWP_NOMOVE  Retains the current position (ignores X and Y parameters).SWP_NOOWNERZORDER Does not change the owner window's position in the Z order.SWP_NOREDRAW Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.SWP_NOREPOSITION Same as the SWP_NOOWNERZORDER flag.SWP_NOSENDCHANGING Prevents the window from receiving the WM_WINDOWPOSCHANGING message.SWP_NOSIZE Retains the current size (ignores the cx and cy parameters).SWP_NOZORDER Retains the current Z order (ignores the hWndInsertAfter parameter).SWP_SHOWWINDOW Displays the window.</param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, Win32Constants.SetWindowPosEnum setWindowPosEnum);
            #endregion
            #region SHowWindow
            public enum ShowWindowEnum {
                /// <summary>
                /// Hides the window and activates another window.
                /// </summary>
                SW_HIDE = 0,
                /// <summary>
                /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
                /// </summary>
                SW_SHOWNORMAL = 1,
                /// <summary>
                /// 
                /// </summary>
                SW_NORMAL = 1,
                /// <summary>
                /// Activates the window and displays it as a minimized window.
                /// </summary>
                SW_SHOWMINIMIZED = 2,
                /// <summary>
                /// Activates the window and displays it as a maximized window.
                /// </summary>
                SW_SHOWMAXIMIZED = 3,
                /// <summary>
                /// Maximizes the specified window.
                /// </summary>
                SW_MAXIMIZE = 3,
                /// <summary>
                /// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except the window is not actived.
                /// </summary>
                SW_SHOWNOACTIVATE = 4,
                /// <summary>
                /// Activates the window and displays it in its current size and position. 
                /// </summary>
                SW_SHOW = 5,
                /// <summary>
                /// Minimizes the specified window and activates the next top-level window in the Z order.
                /// </summary>
                SW_MINIMIZE = 6,
                /// <summary>
                /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
                /// </summary>
                SW_SHOWMINNOACTIVE = 7,
                /// <summary>
                /// Displays the window in its current size and position. This value is similar to SW_SHOW, except the window is not activated.
                /// </summary>
                SW_SHOWNA = 8,
                /// <summary>
                /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
                /// </summary>
                SW_RESTORE = 9,
                /// <summary>
                /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application. 
                /// </summary>
                SW_SHOWDEFAULT = 10,
                /// <summary>
                /// Windows 2000/XP: Minimizes a window, even if the thread that owns the window is hung. This flag should only be used when minimizing windows from a different thread.
                /// </summary>
                SW_FORCEMINIMIZE = 11,
                /// <summary>
                /// 
                /// </summary>
                SW_MAX = 11
            }

            /// <summary>
            /// The ShowWindow function sets the specified window's show state. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window. </param>
            /// <param name="showWindowEnum">[in] Specifies how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values. SW_FORCEMINIMIZE Windows 2000/XP: Minimizes a window, even if the thread that owns the window is hung. This flag should only be used when minimizing windows from a different thread.SW_HIDE Hides the window and activates another window.SW_MAXIMIZE Maximizes the specified window.SW_MINIMIZE Minimizes the specified window and activates the next top-level window in the Z order.SW_RESTORE Activates and displays the window. 
            /// If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.SW_SHOW Activates the window and displays it in its current size and position. SW_SHOWDEFAULT Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application. SW_SHOWMAXIMIZED Activates the window and displays it as a maximized window.SW_SHOWMINIMIZED Activates the window and displays it as a minimized window.SW_SHOWMINNOACTIVE Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.SW_SHOWNA Displays the window in its current size and position. This value is similar to SW_SHOW, except the window is not activated.SW_SHOWNOACTIVATE Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except the window is not actived.SW_SHOWNORMAL Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.</param>
            /// <returns>If the window was previously visible, the return value is nonzero. If the window was previously hidden, the return value is zero. </returns>
            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum showWindowEnum);
            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
            #endregion
        }

        public class WindowHandles {
            [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
            public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
            /// <summary>
            /// The FindWindowEx function retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows, beginning with the one following the specified child window. This function does not perform a case-sensitive search. 
            /// </summary>
            /// <param name="hwndParent">[in] Handle to the parent window whose child windows are to be searched. If hwndParent is NULL, the function uses the desktop window as the parent window. The function searches among windows that are child windows of the desktop. Microsoft® Windows® 2000 and Windows XP: If hwndParent is HWND_MESSAGE, the function searches all message-only windows. </param>
            /// <param name="hwndChildAfter">[in] Handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window. If hwndChildAfter is NULL, the search begins with the first child window of hwndParent. Note that if both hwndParent and hwndChildAfter are NULL, the function searches all top-level and message-only windows. </param>
            /// <param name="lpszClass">Pointer to a null-terminated string that specifies the class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.If lpszClass is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names, or it can be MAKEINTATOM(0x800). In this latter case, 0x8000 is the atom for a menu class. For more information, see the Remarks section of this topic.</param>
            /// <param name="lpszWindow">[in] Pointer to a null-terminated string that specifies the window name (the window's title). If this parameter is NULL, all window names match. </param>
            /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class and window names.If the function fails, the return value is NULL</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

            /// <summary>
            /// The FindWindow function retrieves a handle to the top-level window whose class name and window name match the specified strings. This function does not search child windows. This function does not perform a case-sensitive search.
            /// </summary>
            /// <param name="lpClassName">[in] Pointer to a null-terminated string that specifies the class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero. If lpClassName points to a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names. If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter. </param>
            /// <param name="lpWindowName">[in] Pointer to a null-terminated string that specifies the window name (the window's title). If this parameter is NULL, all window names match. </param>
            /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class name and window name.</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            /// <summary>
            /// The GetDesktopWindow function returns the handle of the Windows desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which all icons and other windows are painted.
            /// </summary>
            /// <returns>The return value is the handle of the desktop window.</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
        }

        public class WindowFocus {
            /// <summary>
            /// The BringWindowToTop function brings the specified window to the top of the Z order. If the window is a top-level window, it is activated. If the window is a child window, the top-level parent window associated with the child window is activated. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window to bring to the top of the Z order. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool BringWindowToTop(IntPtr hWnd);

            /// <summary>
            /// The SetForegroundWindow function puts the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window that should be activated and brought to the foreground. </param>
            /// <returns>If the window was brought to the foreground, the return value is nonzero.If the window was not brought to the foreground, the return value is zero. </returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            /// <summary>
            /// The SetFocus function sets the keyboard focus to the specified window. The window must be attached to the calling thread's message queue
            /// </summary>
            /// <param name="hwnd">[in] Handle to the window that will receive the keyboard input. If this parameter is NULL, keystrokes are ignored</param>
            /// <returns>If the function succeeds, the return value is the handle to the window that previously had the keyboard focus. If the hWnd parameter is invalid or the window is not attached to the calling thread's message queue, the return value is NULL</returns>
            [DllImport("user32.dll")]
            public static extern int SetFocus(int hwnd);

            /// <summary>
            /// The GetForegroundWindow function returns a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads. 
            /// </summary>
            /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation</returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            /// <summary>
            /// The GetFocus function retrieves the handle to the window that has the keyboard focus, if the window is attached to the calling thread's message queue. 
            /// </summary>
            /// <remarks>GetFocus returns the window with the keyboard focus for the current thread's message queue. If GetFocus returns NULL, another thread's queue may be attached to a window that has the keyboard focus. Use the GetForegroundWindow function to retrieve the handle to the window with which the user is currently working. You can associate your thread's message queue with the windows owned by another thread by using the AttachThreadInput function. Windows 98/Me and Windows NT 4.0 SP3 and later:To get the window with the keyboard focus on the foreground queue or the queue of another thread, use the GetGUIThreadInfo function.For an example, see "Creating a Combo Box Toolbar" in Using Combo Boxes.</remarks>
            /// <returns>The return value is the handle to the window with the keyboard focus. If the calling thread's message queue does not have an associated window with the keyboard focus, the return value is NULL. </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetFocus();

        }

        public class Process {
            /// <summary>
            /// The GetWindowThreadProcessId function retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window. </param>
            /// <param name="processId">[out] Pointer to a variable that receives the process identifier. If this parameter is not NULL, GetWindowThreadProcessId copies the identifier of the process to the variable; otherwise, it does not. </param>
            /// <returns>The return value is the identifier of the thread that created the window. </returns>
            [DllImport("user32.dll",SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

            /// <summary>
            /// This function terminates the specified process and all of its threads.
            /// </summary>
            /// <param name="hProcess">[in] Handle to the process to terminate. </param>
            /// <param name="uExitCode">[in] Specifies the exit code for the process and for all threads terminated as a result of this call. Use the GetExitCodeProcess function to retrieve the process's exit value. Use the GetExitCodeThread function to retrieve a thread's exit value. </param>
            /// <returns>Nonzero indicates success. Zero indicates failure</returns>
            [DllImport("kernel32.dll")]
            public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
        }

        public class Thread {
            [DllImport("kernel32.dll")]
            public static extern uint GetCurrentThreadId();
            /// <summary>
            /// This function returns a pseudohandle for the current thread. 
            /// </summary>
            /// <returns>The return value is a pseudohandle for the current thread. </returns>
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetCurrentThread();

            /// <summary>
            /// The AttachThreadInput function attaches or detaches the input processing mechanism of one thread to that of another thread.
            /// </summary>
            /// <param name="idAttach">[in] Identifier of the thread to be attached to another thread. The thread to be attached cannot be a system thread. </param>
            /// <param name="idAttachTo">[in] Identifier of the thread to which idAttach will be attached. This thread cannot be a system thread. A thread cannot attach to itself. Therefore, idAttachTo cannot equal idAttach.</param>
            /// <param name="fAttach">[in] If this parameter is TRUE, the two threads are attached. If the parameter is FALSE, the threads are detached. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
            [DllImport("user32.dll",SetLastError = true)]
            public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        }

        public class Rect {
            /// <summary>
            /// The GetWindowRect function retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen. 
            /// </summary>
            /// <param name="hWnd">[in] Handle to the window.</param>
            /// <param name="lpRect">[out] Pointer to a structure that receives the screen coordinates of the upper-left and lower-right corners of the window. </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero.</returns>
            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, out Win32Types.RECT lpRect);
        }

        public class MouseCursor {
            [DllImport("user32.dll")]
            public static extern bool DrawIcon(IntPtr hDC, int x, int y, IntPtr hIcon);
            [StructLayout(LayoutKind.Sequential)]
            public struct CURSORINFO {
                public Int32 cbSize;
                public readonly Int32 flags;
                public readonly IntPtr hCursor;
                public POINTAPI ptScreenPos;
            }

            [DllImport("user32.dll")]
            public static extern bool GetCursorInfo(out CURSORINFO pci);

            public const Int32 CURSOR_SHOWING = 0x00000001;

            [StructLayout(LayoutKind.Sequential)]
            public struct POINTAPI {
                public readonly int x;
                public readonly int y;
            }

        }

        public class Printers {
            /// <summary>
            /// The SetDefaultPrinter function sets the printer name of the default printer for the current user on the local computer. 
            /// </summary>
            /// <param name="name">[in] Pointer to a null-terminated string containing the default printer name. For a remote printer, the name format is \\server\printername. For a local printer, the name format is printername. If this parameter is NULL or an empty string, that is, "", SetDefaultPrinter does nothing if there is already a default printer. However, if there is no default printer, SetDefaultPrinter sets the default printer to the first printer, if any, in an enumeration of printers installed on the local computer. </param>
            /// <returns>If the function succeeds, the return value is a nonzero value.If the function fails, the return value is zero</returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDefaultPrinter(string name);

            /// <summary>
            /// The GetDefaultPrinter function retrieves the printer name of the default printer for the current user on the local computer. 
            /// </summary>
            /// <param name="pszBuffer">[in] Pointer to a buffer that receives a null-terminated character string containing the default printer name. If this parameter is NULL, the function fails and the variable pointed to by pcchBuffer returns the required buffer size, in characters. </param>
            /// <param name="size">[in/out] On input, specifies the size, in characters, of the pszBuffer buffer. On output, receives the size, in characters, of the printer name string, including the terminating null character. </param>
            /// <returns>If the function succeeds, the return value is a nonzero value and the variable pointed to by pcchBuffer contains the number of characters copied to the pszBuffer buffer, including the terminating null character.If the function fails, the return value is zero</returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int size);
        }

        public class Display {
            /// <summary>
            /// The ChangeDisplaySettings function changes the settings of the default display device to the specified graphics mode. To change the settings of a specified display device, use the ChangeDisplaySettingsEx function
            /// </summary>
            /// <param name="devMode">[in] Pointer to a DEVMODE structure that describes the new graphics mode. If lpDevMode is NULL, all the values currently in the registry will be used for the display setting. Passing NULL for the lpDevMode parameter and 0 for the dwFlags parameter is the easiest way to return to the default mode after a dynamic mode change. The dmSize member of DEVMODE must be initialized to the size, in bytes, of the DEVMODE structure. The dmDriverExtra member of DEVMODE must be initialized to indicate the number of bytes of private driver data following the DEVMODE structure. In addition, you can use any or all of the following members of the DEVMODE structure. </param>
            /// <param name="flags">[in] Indicates how the graphics mode should be changed. This parameter can be one of the following values.</param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern Win32Constants.ChangeDisplaySettingsResult ChangeDisplaySettings(ref Win32Types.DEVMODE devMode, Win32Constants.ChangeDisplaySettingsFlags flags);

            /// <summary>
            /// The EnumDisplaySettings function retrieves information about one of the graphics modes for a display device. To retrieve information for all the graphics modes of a display device, make a series of calls to this function.
            /// </summary>
            /// <param name="deviceName">[in] Pointer to a null-terminated string that specifies the display device about whose graphics mode the function will obtain information. This parameter is either NULL or a DISPLAY_DEVICE.DeviceName returned from EnumDisplayDevices. A NULL value specifies the current display device on the computer on which the calling thread is running. Windows 95: lpszDeviceName must be NULL.</param>
            /// <param name="modeNum">[in] Specifies the type of information to retrieve. This value can be a graphics mode index or one of the following values. Graphics mode indexes start at zero. To obtain information for all of a display device's graphics modes, make a series of calls to EnumDisplaySettings, as follows: Set iModeNum to zero for the first call, and increment iModeNum by one for each subsequent call. Continue calling the function until the return value is zero. When you call EnumDisplaySettings with iModeNum set to zero, the operating system initializes and caches information about the display device. When you call EnumDisplaySettings with iModeNum set to a non-zero value, the function returns the information that was cached the last time the function was called with iModeNum set to zero. </param>
            /// <param name="devMode">[out] Pointer to a DEVMODE structure into which the function stores information about the specified graphics mode. Before calling EnumDisplaySettings, set the dmSize member to sizeof(DEVMODE), and set the dmDriverExtra member to indicate the size, in bytes, of the additional space available to receive private driver data. The EnumDisplaySettings function sets values for the following five DEVMODE members: dmBitsPerPel dmPelsWidth dmPelsHeight dmDisplayFlags dmDisplayFrequency </param>
            /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. </returns>
            [DllImport("user32.dll")]
            public static extern int EnumDisplaySettings(string deviceName, Win32Constants.EnumDisplaySettings modeNum, ref Win32Types.DEVMODE devMode);

        }

        public class GDI32 {
            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            /// <summary>
            /// The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context. 
            /// </summary>
            /// <param name="hdc">[in] Handle to the destination device context. </param>
            /// <param name="nXDest">[in] Specifies the x-coordinate, in logical units, of the upper-left corner of the destination rectangle. </param>
            /// <param name="nYDest">[in] Specifies the y-coordinate, in logical units, of the upper-left corner of the destination rectangle. </param>
            /// <param name="nWidth">[in] Specifies the width, in logical units, of the source and destination rectangles. </param>
            /// <param name="nHeight">[in] Specifies the height, in logical units, of the source and the destination rectangles. </param>
            /// <param name="hdcSrc">[in] Handle to the source device context. </param>
            /// <param name="nXSrc">[in] Specifies the x-coordinate, in logical units, of the upper-left corner of the source rectangle. </param>
            /// <param name="nYSrc">[in] Specifies the y-coordinate, in logical units, of the upper-left corner of the source rectangle. </param>
            /// <param name="dwRop">[in] Specifies a raster-operation code. These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color. </param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth,
                                             int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

            /// <summary>
            /// The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context. 
            /// </summary>
            /// <param name="hObject">[in] Handle to the destination device context. </param>
            /// <param name="nXDest">[in] Specifies the x-coordinate, in logical units, of the upper-left corner of the destination rectangle. </param>
            /// <param name="nYDest">[in] Specifies the y-coordinate, in logical units, of the upper-left corner of the destination rectangle. </param>
            /// <param name="nWidth">[in] Specifies the width, in logical units, of the source and destination rectangles. </param>
            /// <param name="nHeight">[in] Specifies the height, in logical units, of the source and the destination rectangles. </param>
            /// <param name="hObjSource">[in] Handle to the source device context. </param>
            /// <param name="nXSrc">[in] Specifies the x-coordinate, in logical units, of the upper-left corner of the source rectangle. </param>
            /// <param name="nYSrc">[in] Specifies the y-coordinate, in logical units, of the upper-left corner of the source rectangle. </param>
            /// <param name="dwRop">[in] Specifies a raster-operation code. These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color. </param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
                                             int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, Win32Constants.TernaryRasterOperations dwRop);

        }

        public class IniFiles {
            /// <summary>
            /// The GetPrivateProfileInt function retrieves an integer associated
            /// with a key in the specified section of an initialization file
            /// </summary>
            /// <param name="lpAppName">[in] Pointer to a null-terminated string
            /// specifying the name of the section in the initialization file.</param>
            /// <param name="lpKeyName">in] Pointer to the null-terminated string
            /// specifying the name of the key whose value is to be retrieved.
            /// This value is in the form of a string; the GetPrivateProfileInt
            /// function converts the string into an integer and returns the integer.</param>
            /// <param name="nDefault">[in] Default value to return if the key name
            /// cannot be found in the initialization file.</param>
            /// <param name="lpFileName">[in] Pointer to a null-terminated string
            /// that specifies the name of the initialization file. If this parameter
            /// does not contain a full path to the file, the system searches for the
            /// file in the Windows directory.</param>
            /// <returns>The return value is the integer equivalent of the string following
            /// the specified key name in the specified initialization file. If the key
            /// is not found, the return value is the specified default value.</returns>
            /// <remarks>
            /// This method is imported from kernel32.dll.<para/>
            /// 
            /// For more information, check out the MSDN Documentation for
            /// <see href="http://msdn.microsoft.com/library/en-us/sysinfo/base/getprivateprofileint.asp?frame=true">GetPrivateProfileInt</see>.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Int32 GetPrivateProfileInt(
                String lpAppName,
                String lpKeyName,
                Int32 nDefault,
                String lpFileName);

            /// <summary>
            /// The GetPrivateProfileSection function retrieves all the keys and values for
            /// the specified section of an initialization file.
            /// </summary>
            /// <param name="lpAppName">[in] Pointer to a null-terminated string specifying
            /// the name of the section in the initialization file.</param>
            /// <param name="lpReturnedString">[out] Pointer to a buffer that receives the
            /// key name and value pairs associated with the named section. The buffer is
            /// filled with one or more null-terminated strings; the last string is followed
            /// by a second null character.</param>
            /// <param name="nSize">[in] Size of the buffer pointed to by the lpReturnedString
            /// parameter, in TCHARs.</param>
            /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies
            /// the name of the initialization file. If this parameter does not contain a full
            /// path to the file, the system searches for the file in the Windows directory.</param>
            /// <returns>The return value specifies the number of characters copied to the
            /// buffer, not including the terminating null character. If the buffer is not
            /// large enough to contain all the key name and value pairs associated with the
            /// named section, the return value is equal to nSize minus two.</returns>
            /// <remarks>
            /// This method is imported from kernel32.dll.<para/>
            /// 
            /// For more information, check out the MSDN Documentation for
            /// <see href="http://msdn.microsoft.com/library/en-us/sysinfo/base/getprivateprofilesection.asp?frame=true">GetPrivateProfileSection</see>.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Int32 GetPrivateProfileSection(
                String lpAppName,
                Byte[] lpReturnedString,
                Int32 nSize,
                String lpFileName);

            /// <summary>
            /// The GetPrivateProfileSectionNames function retrieves the names of all sections
            /// in an initialization file.
            /// </summary>
            /// <param name="lpszReturnBuffer">[out] Pointer to a buffer that receives the section
            /// names associated with the named file. The buffer is filled with one or more
            /// null-terminated strings; the last string is followed by a second null character.</param>
            /// <param name="nSize">[in] Size of the buffer pointed to by the lpszReturnBuffer
            /// parameter, in TCHARs.</param>
            /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies
            /// the name of the initialization file. If this parameter is NULL, the function
            /// searches the Win.ini file. If this parameter does not contain a full path to the
            /// file, the system searches for the file in the Windows directory.</param>
            /// <returns>The return value specifies the number of characters copied to the
            /// specified buffer, not including the terminating null character. If the buffer
            /// is not large enough to contain all the section names associated with the
            /// specified initialization file, the return value is equal to the size specified
            /// by nSize minus two.</returns>
            /// <remarks>
            /// This method is imported from kernel32.dll.<para/>
            /// 
            /// For more information, check out the MSDN Documentation for
            /// <see href="http://msdn.microsoft.com/library/en-us/sysinfo/base/getprivateprofilesectionnames.asp?frame=true">GetPrivateProfileSectionNames</see>.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Int32 GetPrivateProfileSectionNames(
                Byte[] lpszReturnBuffer,
                Int32 nSize,
                String lpFileName);

            /// <summary>
            /// The GetPrivateProfileString function retrieves a string from the specified
            /// section in an initialization file.
            /// </summary>
            /// <param name="lpAppName">[in] Pointer to a null-terminated string that specifies
            /// the name of the section containing the key name. If this parameter is NULL, the
            /// GetPrivateProfileString function copies all section names in the file to the
            /// supplied buffer.</param>
            /// <param name="lpKeyName">[in] Pointer to the null-terminated string specifying the
            /// name of the key whose associated string is to be retrieved. If this parameter is
            /// NULL, all key names in the section specified by the lpAppName parameter are copied
            /// to the buffer specified by the lpReturnedString parameter.</param>
            /// <param name="lpDefault">[in] Pointer to a null-terminated default string. If the
            /// lpKeyName key cannot be found in the initialization file, GetPrivateProfileString
            /// copies the default string to the lpReturnedString buffer. This parameter cannot be
            /// NULL. <para/>
            /// 
            /// Avoid specifying a default string with trailing blank characters. The function
            /// inserts a null character in the lpReturnedString buffer to strip any trailing
            /// blanks.<para/>
            /// 
            /// <b>Windows Me/98/95:</b>  Although lpDefault is declared as a constant parameter, the system
            /// strips any trailing blanks by inserting a null character into the lpDefault string
            /// before copying it to the lpReturnedString buffer.</param>
            /// <param name="lpReturnedString">[out] Pointer to the buffer that receives the
            /// retrieved string.<para/>
            /// 
            /// <b>Windows Me/98/95:</b>  The string cannot contain control characters (character code
            /// less than 32). Strings containing control characters may be truncated.</param>
            /// <param name="nSize">[in] Size of the buffer pointed to by the lpReturnedString
            /// parameter, in TCHARs.</param>
            /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies
            /// the name of the initialization file. If this parameter does not contain a full
            /// path to the file, the system searches for the file in the Windows directory.</param>
            /// <returns>The return value is the number of characters copied to the buffer, not
            /// including the terminating null character.<para/>
            /// 
            /// If neither lpAppName nor lpKeyName is NULL and the supplied destination buffer is
            /// too small to hold the requested string, the string is truncated and followed by a
            /// null character, and the return value is equal to nSize minus one.<para/>
            /// 
            /// If either lpAppName or lpKeyName is NULL and the supplied destination buffer is too
            /// small to hold all the strings, the last string is truncated and followed by two null
            /// characters. In this case, the return value is equal to nSize minus two.
            /// </returns>
            /// <remarks>
            /// This method is imported from kernel32.dll.
            /// 
            /// For more information, check out the MSDN Documentation for
            /// <see href="http://msdn.microsoft.com/library/en-us/sysinfo/base/getprivateprofilestring.asp?frame=true">GetPrivateProfileString</see>.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Int32 GetPrivateProfileString(
                String lpAppName,
                String lpKeyName,
                String lpDefault,
                StringBuilder lpReturnedString,
                Int32 nSize,
                String lpFileName);

            /// <summary>
            /// The WritePrivateProfileSection function replaces the keys and values for the
            /// specified section in an initialization file.
            /// </summary>
            /// <param name="lpAppName">[in] Pointer to a null-terminated string specifying the
            /// name of the section in which data is written. This section name is typically the
            /// name of the calling application.</param>
            /// <param name="lpString">[in] Pointer to a buffer containing the new key names and
            /// associated values that are to be written to the named section. This string is
            /// limited to 65,535 bytes.</param>
            /// <param name="lpFileName">[in] Pointer to a null-terminated string containing the
            /// name of the initialization file. If this parameter does not contain a full path
            /// for the file, the function searches the Windows directory for the file. If the
            /// file does not exist and lpFileName does not contain a full path, the function
            /// creates the file in the Windows directory. The function does not create a file
            /// if lpFileName contains the full path and file name of a file that does not exist.</param>
            /// <returns>If the function succeeds, the return value is nonzero.<para/>
            /// 
            /// If the function fails, the return value is zero. To get extended error
            /// information, call GetLastError.
            /// </returns>
            /// <remarks>
            /// This method is imported from kernel32.dll.<para/>
            /// 
            /// For more information, check out the MSDN Documentation for
            /// <see href="http://msdn.microsoft.com/library/en-us/sysinfo/base/writeprivateprofilesection.asp?frame=true">WritePrivateProfileSection</see>.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Int32 WritePrivateProfileSection(
                String lpAppName,
                String lpString,
                String lpFileName);

            /// <summary>
            /// The WritePrivateProfileString function copies a string into the specified
            /// section of an initialization file.
            /// </summary>
            /// <param name="lpAppName">[in] Pointer to a null-terminated string containing the
            /// name of the section to which the string will be copied. If the section does not
            /// exist, it is created. The name of the section is case-independent; the string
            /// can be any combination of uppercase and lowercase letters.</param>
            /// <param name="lpKeyName">[in] Pointer to the null-terminated string containing
            /// the name of the key to be associated with a string. If the key does not exist
            /// in the specified section, it is created. If this parameter is NULL, the entire
            /// section, including all entries within the section, is deleted.</param>
            /// <param name="lpString">[in] Pointer to a null-terminated string to be written to
            /// the file. If this parameter is NULL, the key pointed to by the lpKeyName
            /// parameter is deleted.<para/>
            /// 
            /// <b>Windows Me/98/95:</b> The system does not support the use of the TAB (\t) character
            /// as part of this parameter.</param>
            /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies
            /// the name of the initialization file.</param>
            /// <returns>If the function successfully copies the string to the initialization file,
            /// the return value is nonzero.<para/>
            /// 
            /// If the function fails, or if it flushes the cached version of the most recently
            /// accessed initialization file, the return value is zero. To get extended error
            /// information, call GetLastError.</returns>
            /// <remarks>
            /// This method is imported from kernel32.dll.<para/>
            /// 
            /// For more information, check out the MSDN Documentation for
            /// <see href="http://msdn.microsoft.com/library/en-us/sysinfo/base/writeprivateprofilestring.asp?frame=true">WritePrivateProfileString</see>.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Int32 WritePrivateProfileString(
                String lpAppName,
                String lpKeyName,
                String lpString,
                String lpFileName);

            /// <summary>
            /// The GetProfileString function retrieves the string associated with a key in the specified section of the Win.ini file.Note  This function is provided only for compatibility with 16-bit Windows-based applications, therefore this function should not be called from server code. Applications should store initialization information in the registry.
            /// </summary>
            /// <param name="lpAppName">[in] Pointer to a null-terminated string that specifies the name of the section containing the key. If this parameter is NULL, the function copies all section names in the file to the supplied buffer.</param>
            /// <param name="lpKeyName">[in] Pointer to a null-terminated string specifying the name of the key whose associated string is to be retrieved. If this parameter is NULL, the function copies all keys in the given section to the supplied buffer. Each string is followed by a null character, and the final string is followed by a second null character. </param>
            /// <param name="lpDefault">[in] Pointer to a null-terminated default string. If the lpKeyName key cannot be found in the initialization file, GetProfileString copies the default string to the lpReturnedString buffer. This parameter cannot be NULL. Avoid specifying a default string with trailing blank characters. The function inserts a null character in the lpReturnedString buffer to strip any trailing blanks.Windows Me/98/95:  Although lpDefault is declared as a constant parameter, the system strips any trailing blanks by inserting a null character into the lpDefault string before copying it to the lpReturnedString buffer.</param>
            /// <param name="lpReturnedString">[out] Pointer to a buffer that receives the character string.</param>
            /// <param name="nSize">[in] Size of the buffer pointed to by the lpReturnedString parameter, in TCHARs. </param>
            /// <returns>The return value is the number of characters copied to the buffer, not including the null-terminating character.If neither lpAppName nor lpKeyName is NULL and the supplied destination buffer is too small to hold the requested string, the string is truncated and followed by a null character, and the return value is equal to nSize minus one.If either lpAppName or lpKeyName is NULL and the supplied destination buffer is too small to hold all the strings, the last string is truncated and followed by two null characters. In this case, the return value is equal to nSize minus two.</returns>
            [DllImport("kernel32.dll")]
            public static extern uint GetProfileString(string lpAppName, string lpKeyName,
                                                       string lpDefault, [Out] StringBuilder lpReturnedString, uint nSize);


        }
    }
}