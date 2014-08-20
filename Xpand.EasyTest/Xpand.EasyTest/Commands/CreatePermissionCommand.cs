using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class CreatePermissionCommand:Command{
        public const string Name = "CreatePermission";
        protected override void InternalExecute(ICommandAdapter adapter){
            NavigateToRole(adapter);
            ProccessUserRole(adapter);
            CreateNewTypePermission(adapter);
            NavigateToRole(adapter);
        }

        private void CreateNewTypePermission(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Type Permissions");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
            actionCommand.Parameters.MainParameter = new MainParameter("Type Permissions.New");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);

            var fillFormCommand = new FillFormCommand();
            fillFormCommand.Parameters.Add(new Parameter("Target Type", Parameters.MainParameter.Value, true, StartPosition));
            fillFormCommand.Parameters.Add(new Parameter("Read", this.ParameterValue("Read", true.ToString()), true, StartPosition));
            fillFormCommand.Parameters.Add(new Parameter("Write", this.ParameterValue("Write", true.ToString()), true, StartPosition));
            fillFormCommand.Parameters.Add(new Parameter("Delete", this.ParameterValue("Delete", true.ToString()), true, StartPosition));
            fillFormCommand.Parameters.Add(new Parameter("Create", this.ParameterValue("Create", true.ToString()), true, StartPosition));
            fillFormCommand.Parameters.Add(new Parameter("Navigate", this.ParameterValue("Navigate", true.ToString()), true, StartPosition));
            fillFormCommand.Execute(adapter);

            var saveAndCloseCommand = new SaveAndCloseCommand();
            saveAndCloseCommand.Execute(adapter);

            if (adapter.IsWinAdapter()){
                saveAndCloseCommand = new SaveAndCloseCommand();
                saveAndCloseCommand.Execute(adapter);
            }
        }

        private void ProccessUserRole(ICommandAdapter adapter){
            var processRecordCommand = new DevExpress.EasyTest.Framework.Commands.ProcessRecordCommand();
            processRecordCommand.Parameters.MainParameter=new MainParameter("");
            processRecordCommand.Parameters.Add(new Parameter("Name", "User", true, StartPosition));
            processRecordCommand.Execute(adapter);
        }

        private void NavigateToRole(ICommandAdapter adapter){
            var navigateCommand = new NavigateCommand();
            navigateCommand.Parameters.MainParameter = new MainParameter(this.ParameterValue("Role", "Default.Role"));
            navigateCommand.Execute(adapter);
        }
    }
}
