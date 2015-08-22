using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandFillRecordCommand:Command{
        public const string Name = "XpandFillRecord";
        protected override void InternalExecute(ICommandAdapter adapter){
            SelectRecord(adapter);
            ExecuteTableAction(adapter, "InlineEdit");
            var fillRecordCommand = new FillRecordCommand();
            fillRecordCommand.Parameters.MainParameter = Parameters.MainParameter;
            fillRecordCommand.Parameters.AddRange(Parameters);
            fillRecordCommand.Execute(adapter);
            ExecuteTableAction(adapter, "InlineUpdate");
        }

        private void SelectRecord(ICommandAdapter adapter){
            var selectRecordColumnParameter = Parameters["SelectColumn"];
            if (selectRecordColumnParameter != null){
                var clearSelectionCommand = new ClearSelectionCommand();
                clearSelectionCommand.Parameters.MainParameter = Parameters.MainParameter;
                clearSelectionCommand.Execute(adapter);
                var selectRecordsCommand = new SelectRecordsCommand();
                selectRecordsCommand.Parameters.MainParameter = Parameters.MainParameter;
                selectRecordsCommand.Parameters.Add(new Parameter(" Columns = " + selectRecordColumnParameter.Value, EndPosition));
                var selectRecordRowParameter = Parameters["SelectRow"];
                selectRecordsCommand.Parameters.Add(new Parameter(" Row = "+ selectRecordRowParameter.Value,  EndPosition));
                selectRecordsCommand.Execute(adapter);
            }
        }

        private void ExecuteTableAction(ICommandAdapter adapter,string action){
            var executeTableActionCommand = new ExecuteTableActionCommand();
            executeTableActionCommand.Parameters.MainParameter = Parameters.MainParameter;
            executeTableActionCommand.Parameters.Add(new Parameter(" "+action+" = ''",EndPosition));
            executeTableActionCommand.Execute(adapter);
        }
    }
}