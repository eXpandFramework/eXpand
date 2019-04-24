using System.Drawing;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands.InputSimulator{
    public class MouseDragDropCommand:Command{
        public const string Name = "MouseDragDrop";
        protected override void InternalExecute(ICommandAdapter adapter){
            var inputSimulator = new Utils.Automation.InputSimulator.InputSimulator();
            inputSimulator.Mouse.DragAndDrop(this.ParameterValue<Point>("Start"), this.ParameterValue<Point>("End"));
        }
    }
}