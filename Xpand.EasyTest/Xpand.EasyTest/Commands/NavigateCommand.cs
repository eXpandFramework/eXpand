using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class NavigateCommand:Command{
        public const string Name = "Navigate";
        protected override void InternalExecute(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            var parameterList = actionCommand.Parameters;
            parameterList.MainParameter=new MainParameter("Navigation");
            parameterList.ExtraParameter = Parameters.MainParameter;
            actionCommand.Execute(adapter);
        }
    }
}