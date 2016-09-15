using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class DeleteAllObjectsCommand:Command{
        public const string Name = "DeleteAllObjects";
        protected override void InternalExecute(ICommandAdapter adapter){
            var executeTableActionCommand = new ExecuteTableActionCommand();
            var mainParameter = new MainParameter{Value = ""};
            executeTableActionCommand.Parameters.MainParameter = mainParameter;
            executeTableActionCommand.Parameters.Add(new Parameter("SelectAll","True",true, EndPosition));
            executeTableActionCommand.Execute(adapter);

            var optionalActionCommand = new OptionalActionCommand();
            optionalActionCommand.DoAction(adapter, "Delete",null);
            optionalActionCommand.DoAction(adapter, "Yes",null);
        }
    }
}
