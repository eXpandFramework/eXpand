using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class SaveAndCloseCommand : Command{
        public const string Name = "SaveAndClose";
        protected override void InternalExecute(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter=new MainParameter("Save and Close");
            actionCommand.Parameters.ExtraParameter=new MainParameter();
            actionCommand.Execute(adapter);
        }
    }
}