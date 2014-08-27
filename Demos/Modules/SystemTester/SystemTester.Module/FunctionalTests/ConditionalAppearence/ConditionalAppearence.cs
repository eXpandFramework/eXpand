using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Utils.Automation;
using Xpand.Utils.Win32;

namespace SystemTester.Module.FunctionalTests.ConditionalAppearence {
    public class ConditionalAppearence : ObjectViewController<ObjectView, ConditionalAppearenceObject> {
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, IntPtr lParam);
        const int EM_SETSEL = 0x00B1;
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        extern static int SendMessageGetTextLength(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        const int WM_GETTEXTLENGTH = 0x000E;

        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<RefreshController>().RefreshAction.Execute+=RefreshActionOnExecute;
        }

        public static string LogWindowText(IntPtr intPtr) {
            int length = Win32Declares.Window.GetWindowTextLength(intPtr);
            var sb = new StringBuilder(length + 1);
            Win32Declares.Window.GetWindowText(intPtr, sb, sb.Capacity);
            var text = sb.ToString();
            return text;
        }

        private void RefreshActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var intPtr = ((Control) View.GetItems<PropertyEditor>().First().Control).Handle;
            var focusControlHandle = new HelperAutomation().GetFocusControlHandle();
            var logWindowText = LogWindowText(focusControlHandle);
            int length = SendMessageGetTextLength(intPtr, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
            SendMessage(focusControlHandle, EM_SETSEL, -1, IntPtr.Zero);
            
//            SendKeys.SendWait("{RIGHT}");
        }
    }
}
