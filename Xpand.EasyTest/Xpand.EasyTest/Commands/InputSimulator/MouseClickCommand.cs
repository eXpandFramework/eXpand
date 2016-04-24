using System.Threading;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands.InputSimulator{
    public abstract class MouseClickCommand:Command{
        protected override void InternalExecute(ICommandAdapter adapter){
            Thread.Sleep(1000);

            var mouseCommand = new MouseCommand();
            mouseCommand.Parameters.MainParameter = new MainParameter(ClickMethodName());
            mouseCommand.Parameters.Add(new Parameter("MoveMouseTo", Parameters.MainParameter.Value, true, EndPosition));
            mouseCommand.Execute(adapter);

            Thread.Sleep(1000);
        }


        protected abstract string ClickMethodName();
    }
}