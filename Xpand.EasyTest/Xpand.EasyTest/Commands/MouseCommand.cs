using WindowsInput;
using DevExpress.EasyTest.Framework;
using Fasterflect;
using Point = System.Drawing.Point;

namespace Xpand.EasyTest.Commands{
    public class MouseCommand:Command{
        public const string Name = "Mouse";
        protected override void InternalExecute(ICommandAdapter adapter){
            var simulator = new InputSimulator();
            var point = this.ParameterValue<Point>("MoveMouseTo");
            simulator.Mouse.MoveMouseTo(point.X, point.Y);
            simulator.Mouse.CallMethod(Parameters.MainParameter.Value);
        }
    }
}