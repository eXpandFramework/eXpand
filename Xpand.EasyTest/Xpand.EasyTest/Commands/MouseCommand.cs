using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Fasterflect;
using Xpand.Utils.Automation.InputSimulator;
using Point = System.Drawing.Point;

namespace Xpand.EasyTest.Commands{
    public class MouseCommand:Command{
        private static readonly InputSimulator _simulator=new InputSimulator();
        public const string Name = "Mouse";
        protected override void InternalExecute(ICommandAdapter adapter){
            var toggleNavigation = this.ParameterValue<bool>(ToggleNavigationCommand.Name);
            if (toggleNavigation){
                var toggleNavigationCommand = new ToggleNavigationCommand();
                toggleNavigationCommand.Execute(adapter);
                var sleepCommand = new SleepCommand();
                sleepCommand.Parameters.MainParameter=new MainParameter("1000");
            }
            try{
                var focusControlCommand = new FocusWindowCommand();
                focusControlCommand.Execute(adapter);
                if (Parameters["MoveMouseTo"]!=null){
                    var point = this.ParameterValue<Point>("MoveMouseTo");
                    _simulator.Mouse.MoveMouseTo(point.X, point.Y);
                    Cursor.Position=point;
                }
                if (!string.IsNullOrEmpty(Parameters.MainParameter.Value)){
                    _simulator.Mouse.CallMethod(Parameters.MainParameter.Value);
                }
            }
            finally{
                if (toggleNavigation) {
                    var toggleNavigationCommand = new ToggleNavigationCommand();
                    toggleNavigationCommand.Execute(adapter);
                }    
            }
        }
    }
}