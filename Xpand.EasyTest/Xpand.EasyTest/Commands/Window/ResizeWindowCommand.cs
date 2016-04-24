using System.Drawing;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands.Window{
    public class ResizeWindowCommand : WindowCommand{
        public const string Name = "ResizeWindow";

        protected override void InternalExecute(ICommandAdapter adapter) { 
            EasyTestTracer.Tracer.InProcedure(Name);
            var value = this.ParameterValue(new Size(1024, 768));
            var windowHandle = GetWindowHandle(adapter);
            windowHandle.ResizeWindow(value);
            EasyTestTracer.Tracer.OutProcedure(Name);
        }
    }
}