using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class LogOffCommand : Command{
        public const string Name = "LogOff";
        protected override void InternalExecute(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Log Off");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
        }
    }
}