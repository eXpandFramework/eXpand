using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class ChangeUserPasswordCommand:Command{
        public const string Name = "ChangeUserPassword";
        protected override void InternalExecute(ICommandAdapter adapter){
            var navigateCommand = new NavigateCommand();
            navigateCommand.Parameters.MainParameter = new MainParameter("Default.My Details");
            navigateCommand.Execute(adapter);
            
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Change My Password");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);

            var fillFormCommand = new FillFormCommand();
            var password = Parameters.MainParameter.Value+"";
            fillFormCommand.Parameters.Add(new Parameter("New Password",password,true,StartPosition));
            fillFormCommand.Parameters.Add(new Parameter("Confirm Password", password, true, StartPosition));
            fillFormCommand.Execute(adapter);

            actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("OK");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
        }
    }
}