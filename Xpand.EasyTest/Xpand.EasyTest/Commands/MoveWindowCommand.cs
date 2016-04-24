using System;
using System.Drawing;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands{
    public class MoveWindowCommand:Command{
        public const string Name = "MoveWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            EasyTestTracer.Tracer.InProcedure(Name);
            var value = this.ParameterValue(new Rectangle(0,0,1024,768));
            var windowHandle = GetWindowHandle(adapter);
            windowHandle.MoveWindow(value);
            EasyTestTracer.Tracer.OutProcedure(Name);
        }

        private IntPtr GetWindowHandle(ICommandAdapter adapter){
            var extraParameter = Parameters.ExtraParameter;
            return (extraParameter != null && extraParameter.Value != null)
                ? new IntPtr(int.Parse(extraParameter.Value))
                : adapter.GetMainWindowHandle();
        }
    }
}