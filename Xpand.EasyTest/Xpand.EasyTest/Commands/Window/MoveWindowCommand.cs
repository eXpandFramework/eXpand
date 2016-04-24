using System.Drawing;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands.Window{
    public class MoveWindowCommand:WindowCommand{
        public const string Name = "MoveWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            EasyTestTracer.Tracer.InProcedure(Name);
            var value = this.ParameterValue(new Rectangle(0,0,1024,768));
            var windowHandle = GetWindowHandle(adapter);
            windowHandle.MoveWindow(value);
            EasyTestTracer.Tracer.OutProcedure(Name);
        }
    }
}