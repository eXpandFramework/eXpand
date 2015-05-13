using System.Drawing;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation.InputSimulator;

namespace Xpand.EasyTest.Commands{
    public class MouseDragDropCommand:Command{
        public const string Name = "MouseDragDrop";
        protected override void InternalExecute(ICommandAdapter adapter){
            var inputSimulator = new InputSimulator();
            inputSimulator.Mouse.DragAndDrop(this.ParameterValue<Point>("Start"), this.ParameterValue<Point>("End"));
        }
    }
}