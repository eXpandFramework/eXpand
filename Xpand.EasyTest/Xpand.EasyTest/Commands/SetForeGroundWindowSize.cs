using System;
using System.Drawing;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Automation;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public class ResizeWindowCommand:Command{
        public const string Name = "ResizeWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            EasyTestTracer.Tracer.InProcedure(Name);
            IntPtr windowHandle = Win32Declares.WindowFocus.GetForegroundWindow();
            var windowSize = SetActiveWindowSizeCommand.GetWindowSize(Parameters.MainParameter.Value);
            WindowAutomation.ResizeWindow(windowHandle, windowSize);
            WindowAutomation.MoveWindow(windowHandle, new Point(0, 0));

            var focusWindowCommand = new FocusWindowCommand();
            focusWindowCommand.Execute(adapter);
            EasyTestTracer.Tracer.OutProcedure(Name);
        }
    }
}