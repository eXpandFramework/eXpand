using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class DeleteAllObjectsCommand:Command, IRequireApplicationOptions {
        private TestApplication _applicationOptions;


        public const string Name = "DeleteAllObjects";
        protected override void InternalExecute(ICommandAdapter adapter){
            var executeTableActionCommand = new ExecuteTableActionCommand();
            executeTableActionCommand.SetApplicationOptions(_applicationOptions);
            var mainParameter = new MainParameter{Value = ""};
            executeTableActionCommand.Parameters.MainParameter = mainParameter;
            executeTableActionCommand.Parameters.Add(new Parameter(" SelectAll = True",EndPosition));
            executeTableActionCommand.Execute(adapter);

            var optionalActionCommand = new OptionalActionCommand();
            optionalActionCommand.DoAction(adapter, "Delete",null);
            optionalActionCommand.DoAction(adapter, "Yes",null);
        }

        public void SetApplicationOptions(TestApplication applicationOptions){
            _applicationOptions = applicationOptions;
        }
    }
}
