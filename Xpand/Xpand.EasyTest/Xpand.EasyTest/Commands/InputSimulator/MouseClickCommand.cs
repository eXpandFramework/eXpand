using System.Drawing;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands.InputSimulator{
    public abstract class MouseClickCommand:Command{
        protected override void InternalExecute(ICommandAdapter adapter){
            PointForm.Show(this.ParameterValue<Point>());
            var mouseCommand = new MouseCommand();
            mouseCommand.Parameters.MainParameter = new MainParameter(ClickMethodName());
            mouseCommand.Parameters.Add(new Parameter("MoveMouseTo", Parameters.MainParameter.Value, true, EndPosition));
            mouseCommand.Execute(adapter);
        }


        protected abstract string ClickMethodName();
    }
}