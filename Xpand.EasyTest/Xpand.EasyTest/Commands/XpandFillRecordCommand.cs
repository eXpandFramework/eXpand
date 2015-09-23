using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class XpandFillRecordCommand : Command {
        public const string Name = "XpandFillRecord";
        protected override void InternalExecute(ICommandAdapter adapter) {
            ExecuteTableAction(adapter, "InlineEdit");
            var fillRecordCommand = new FillRecordCommand();
            fillRecordCommand.Parameters.MainParameter = Parameters.MainParameter;
            fillRecordCommand.Parameters.Add(Parameters["Columns"]);
            fillRecordCommand.Parameters.Add(Parameters["Values"]);
            fillRecordCommand.Execute(adapter);
            ExecuteTableAction(adapter, "InlineUpdate");
        }

        private void ExecuteTableAction(ICommandAdapter adapter, string action) {
            var executeTableActionCommand = new ExecuteTableActionCommand();
            executeTableActionCommand.Parameters.MainParameter = Parameters.MainParameter;
            var parameters = Parameters.Where(parameter => parameter.Name != "Columns" && parameter.Name != "Values").Select(parameter => new Parameter(parameter.Name, parameter.Value,parameter.IsEqual,EndPosition)).First();
            executeTableActionCommand.Parameters.Add(parameters);
            executeTableActionCommand.Parameters.Add(new Parameter(" " + action + " = ''", EndPosition));
            executeTableActionCommand.Execute(adapter);
        }
    }
}