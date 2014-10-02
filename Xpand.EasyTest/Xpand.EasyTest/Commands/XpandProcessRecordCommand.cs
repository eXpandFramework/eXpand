using System;
using System.Collections.Generic;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandProcessRecordCommand : ProcessRecordCommand{
        private ICommandAdapter _adapter;
        public const string Name = "XpandProcessRecord";

        protected override void InternalExecute(ICommandAdapter adapter){
            if (!adapter.IsWinAdapter()&&Parameters["Action"]==null){
                Parameters.Add(new Parameter("Action","Edit",true,StartPosition));
            }
            base.InternalExecute(adapter);
            _adapter = adapter;
        }

        protected override bool IsActionParameterName(string parameterName){
            bool isActionParameterName = base.IsActionParameterName(parameterName);
            if (isActionParameterName&&_adapter.IsWinAdapter()){
                var parameter = Parameters[Parameters.Count - 1];
                if (parameter.Value == "Edit"){
                    Parameters.Remove(parameter);
                    return false;
                }
            }
            return isActionParameterName;
        }
    }

    public abstract class ProcessRecordCommand : GridActionCommandBase{
        protected ProcessRecordCommand(){
            HasMainParameter = true;
        }

        protected override void Act(string actionName, int rowIndex, IGridColumn actionColumn, ITestControl testTable){
            testTable.GetInterface<IGridAct>().GridAct(actionName, rowIndex, actionColumn);
        }
    }

    public abstract class GridActionCommandBase : Command{
        public virtual void ProcessRecord(ICommandAdapter adapter, string tableName, string actionName,
            string[] columnCaptions, string[] cellValues){
            if (!adapter.IsControlExist(TestControlType.Table, tableName)){
                throw new CommandException(
                    string.Format("Cannot find the '{0}' control, OperationTag:'{1}'", tableName, TestControlType.Table),
                    StartPosition);
            }
            ITestControl testTable = adapter.CreateTestControl(TestControlType.Table, tableName);
            var table = testTable.GetInterface<IGridBase>();
            IGridColumn actionColumn = null;
            int rowCount = table.GetRowCount();
            if (rowCount == 0 && cellValues.Length > 0){
                string message = string.IsNullOrEmpty(tableName)
                    ? "The table is empty"
                    : string.Format("The '{0}' table is empty", tableName);
                throw new AdapterOperationException(message);
            }

            int rowIndex = -1;
            if (cellValues.Length > 0){
                var testControl = testTable.FindInterface<IGridRowsSelection>();
                if (testControl != null && !string.IsNullOrEmpty(actionName)){
                    testControl.ClearSelection();
                    //B158392
                    rowIndex = GetRowIndex(testTable, rowCount, columnCaptions, cellValues);
                    testControl.SelectRow(rowIndex);
                }
                else{
                    rowIndex = GetRowIndex(testTable, rowCount, columnCaptions, cellValues);
                }
                if (rowIndex == -1){
                    throw new AdapterOperationException(
                        string.Format("The record with the '{0}' value was not found. Checked values: {1}",
                            string.Join(", ", cellValues), GridControlHelper.GetFormatTableValues(null)));
                }
                var gridHelper = new GridControlHelper(testTable);
                List<IGridColumn> columnList = gridHelper.GetColumnIndexes(columnCaptions, false);
                if (columnList.Count > 0)
                    actionColumn = columnList[0];
            }
            Act(actionName, rowIndex, actionColumn, testTable);
        }

        private int GetRowIndex(ITestControl testTable, int rowCount, string[] columnCaptions,
            string[] cellValues){
            var gridHelper = new GridControlHelper(testTable);
            int rowIndex = -1;
            if (cellValues.Length > 0){
                string[,] tableValues = null;
                List<IGridColumn> columnList = gridHelper.GetColumnIndexes(columnCaptions, false);
                rowIndex = gridHelper.SearchRowIndex(columnList, cellValues, rowCount, ref tableValues);
                if (rowIndex == -1)
                    throw new AdapterOperationException(
                        string.Format("The record with the '{0}' value was not found. Checked values: {1}",
                            string.Join(", ", cellValues), GridControlHelper.GetFormatTableValues(tableValues)));
            }

            return rowIndex;
        }

        protected virtual bool IsActionParameterName(string parameterName){
            return parameterName == "Action";
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            if (Parameters.Count > 0){
                string actionName = String.Empty;
                Parameter actionParam = null;
                if (IsActionParameterName(Parameters[Parameters.Count - 1].Name)){
                    actionParam = Parameters[Parameters.Count - 1];
                    actionName = actionParam.Value;
                }
                var captions = new List<string>();
                var values = new List<string>();
                for (int i = 0; i <= Parameters.Count - (GetActionParametersCount(actionParam) + 1); i++){
                    captions.Add(Parameters[i].Name);
                    values.Add(Parameters[i].Value);
                }
                ProcessRecord(adapter, Parameters.MainParameter.Value, actionName, captions.ToArray(), values.ToArray());
            }
        }

        protected virtual int GetActionParametersCount(Parameter actionParam){
            return actionParam == null ? 0 : 1;
        }

        protected abstract void Act(string actionName, int rowIndex, IGridColumn actionColumn, ITestControl testTable);
    }
}