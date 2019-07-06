using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using Fasterflect;
using Xpand.EasyTest.Commands.Window;
using Point = System.Drawing.Point;

namespace Xpand.EasyTest.Commands.InputSimulator{
    public class MouseCommand:Command{
        private static readonly Automation.InputSimulator.InputSimulator Simulator=new Automation.InputSimulator.InputSimulator();
        public const string Name = "Mouse";
        protected override void InternalExecute(ICommandAdapter adapter){
            var toggleNavigation = this.ParameterValue<bool>(ToggleNavigationCommand.Name);
            var toggleNavigationCommand = new ToggleNavigationCommand();
            if (toggleNavigation){
                toggleNavigationCommand.Execute(adapter);
                Thread.Sleep(500);
            }
            try{
                ActivateAppWindow(adapter);
                if (Parameters["MoveMouseTo"]!=null){
                    var point = this.ParameterValue<Point>("MoveMouseTo");
                    Cursor.Position=point;
                    Simulator.Mouse.MoveMouseTo(point.X, point.Y);
                }
                if (!string.IsNullOrEmpty(Parameters.MainParameter.Value)){
                    Simulator.Mouse.CallMethod(Parameters.MainParameter.Value);
                }
            }
            finally{
                if (toggleNavigation) {
                    toggleNavigationCommand.Execute(adapter);
                    Thread.Sleep(500);
                }    
            }
        }

        private static void ActivateAppWindow(ICommandAdapter adapter){
            try{
                var activateApplicationWindowCommand = new ActivateWindowCommand();
                activateApplicationWindowCommand.Execute(adapter);
            }
            catch{
                // ignored
            }
        }
    }
}