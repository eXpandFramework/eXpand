using System.Windows.Forms;
using WindowsInput;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Fasterflect;
using Point = System.Drawing.Point;

namespace Xpand.EasyTest.Commands{
    public class MouseCommand:Command{
        private static readonly InputSimulator _simulator=new InputSimulator();
        public const string Name = "Mouse";
        protected override void InternalExecute(ICommandAdapter adapter){
            var sleepCommand = new SleepCommand();
            if (Parameters["MoveMouseTo"]!=null){
                var point = this.ParameterValue<Point>("MoveMouseTo");
                Cursor.Position=point;
                sleepCommand.Parameters.MainParameter = new MainParameter("200");
                sleepCommand.Execute(adapter);
            }
            if (!string.IsNullOrEmpty(Parameters.MainParameter.Value)){
                _simulator.Mouse.CallMethod(Parameters.MainParameter.Value);
                sleepCommand.Parameters.MainParameter = new MainParameter("200");
                sleepCommand.Execute(adapter);
            }
        }
    }
}