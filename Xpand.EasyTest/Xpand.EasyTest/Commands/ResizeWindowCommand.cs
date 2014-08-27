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
            var windowHandle = GetWindowHandle();
            var windowSize = SetActiveWindowSizeCommand.GetWindowSize(Parameters.MainParameter.Value);
            if (adapter.IsWinAdapter())
                WindowAutomation.ResizeWindow(windowHandle, windowSize);
            else{
                var setActiveWindowSizeCommand = new SetActiveWindowSizeCommand();
                setActiveWindowSizeCommand.Parameters.MainParameter=Parameters.MainParameter;
                setActiveWindowSizeCommand.Execute(adapter);
            }
            WindowAutomation.MoveWindow(windowHandle, new Point(0, 0));
            EasyTestTracer.Tracer.OutProcedure(Name);
        }

        private IntPtr GetWindowHandle(){
            var extraParameter = Parameters.ExtraParameter;
            return (extraParameter != null && extraParameter.Value!=null) ? new IntPtr(int.Parse(extraParameter.Value)) : Win32Declares.WindowFocus.GetForegroundWindow();
        }
    }
}