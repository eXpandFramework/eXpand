using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Layout;
using Fasterflect;

namespace Xpand.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls{
    public class XpandGridTestControl : GridTestControl, IGridControlAct, IGridDoubleClick, IGridBase,
        IGridRowsSelection, IGridCellControlCreation{
        public XpandGridTestControl(GridControl control) : base(control){
        }

        int IGridBase.GetRowCount(){
            var layoutView = control.DefaultView as LayoutView;
            return layoutView == null ? base.GetRowCount() : layoutView.RowCount;
        }

        string IGridBase.GetCellValue(int row, IGridColumn column){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                return base.GetCellValue(row, column);
            var gridColumn = ((DXGridColumn) column).Column;
            var displayText = layoutView.GetRowCellDisplayText(row, gridColumn);
            var value = layoutView.GetRowCellValue(row, gridColumn);
            if (value is bool){
                if (string.IsNullOrEmpty(displayText) || displayText == "Checked" || displayText == "Unchecked"){
                    displayText = value.ToString();
                }
            }
            return displayText;
        }

        IEnumerable<IGridColumn> IGridBase.Columns{
            get{
                var layoutView = control.DefaultView as LayoutView;
                return layoutView == null
                    ? base.Columns
                    : layoutView.Columns.Cast<GridColumn>().Select(column => new DXGridColumn(column));
            }
        }

        void IGridCellControlCreation.BeginEdit(int row){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                base.BeginEdit(row);
            else{
                layoutView.FocusedRowHandle = row;
                layoutView.BeginInit();
            }
        }

        ITestControl IGridCellControlCreation.CreateCellControl(int row, IGridColumn column){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                return base.CreateCellControl(row, column);
            layoutView.FocusedColumn = ((DXGridColumn) column).Column;
            layoutView.ShowEditor();
            if (layoutView.ActiveEditor == null){
                throw new AdapterOperationException(
                    string.Format("Cannot get the ActiveEditor for the '{0}' table's ({1}, {2}) cell", TestControl.Name,
                        row, column.Caption));
            }
            return TestControlFactoryWin.Instance.CreateControl(layoutView.ActiveEditor);
        }

        void IGridCellControlCreation.EndEdit(){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                base.EndEdit();
            else{
                if (layoutView.ActiveEditor != null){
                    this.CallMethod("RaiseKeyDownEnter");
                }
            }
        }

        void IGridControlAct.GridActEx(string actionName, int rowIndex, IGridColumn column, string[] paramValues){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null || actionName != "SetTableFilter")
                base.GridActEx(actionName, rowIndex, column, paramValues);
            else{
                layoutView.ActiveFilterCriteria = CriteriaOperator.Parse(paramValues[0]);
            }
        }

        void IGridDoubleClick.DoubleClickToCell(int row, IGridColumn column){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                base.DoubleClickToCell(row, column);
        }

        void IGridRowsSelection.ClearSelection(){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                base.ClearSelection();
            else
                layoutView.ClearSelection();
        }

        void IGridRowsSelection.SelectRow(int rowIndex){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                base.SelectRow(rowIndex);
            else{
                layoutView.FocusedRowHandle = rowIndex;
                layoutView.SelectRow(rowIndex);
            }
        }

        void IGridRowsSelection.UnselectRow(int rowIndex){
            var layoutView = control.DefaultView as LayoutView;
            if (layoutView == null)
                base.UnselectRow(rowIndex);
            else{
                layoutView.UnselectRow(rowIndex);
            }
        }

        bool IGridRowsSelection.IsRowSelected(int rowIndex){
            var layoutView = control.DefaultView as LayoutView;
            return layoutView == null ? base.IsRowSelected(rowIndex) : layoutView.IsRowSelected(rowIndex);
        }
    }
}