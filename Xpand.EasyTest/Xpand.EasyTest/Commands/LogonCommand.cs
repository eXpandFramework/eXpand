using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class LogonCommand:Command{
        public const string Name = "LogOn";
        protected override void InternalExecute(ICommandAdapter adapter){
            FillForm(adapter);
            LogOn(adapter);
        }

        private void LogOn(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Log On");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
        }

        private void FillForm(ICommandAdapter adapter){
            var fillFormCommand = new XpandFillFormCommand();
            var userName = GetUserName();
            fillFormCommand.Parameters.Add(new Parameter("User Name", userName, true, StartPosition));
            fillFormCommand.Execute(adapter);
        }

        private string GetUserName(){
            return string.IsNullOrEmpty(Parameters.MainParameter.Value) ? "Admin" : Parameters.MainParameter.Value;
        }
    }
}