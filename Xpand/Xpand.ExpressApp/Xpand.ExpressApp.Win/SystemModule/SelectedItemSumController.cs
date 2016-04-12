using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelMemberSeletectItemSum{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool SelectedItemSum { get; set; } 
    }

    [ModelInterfaceImplementor(typeof(IModelMemberSeletectItemSum),"ModelMember")]
    public interface IModelColumnSelectedItemSum : IModelMemberSeletectItemSum {
         
    }
    public class SelectedItemSumController : ViewController<ListView>,IModelExtender {
        private ToolTipController _toolTipController;
        private GridView _gridView;


        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var columnsListEditor = View.Editor as WinColumnsListEditor;
            if (columnsListEditor != null) {
                _gridView = columnsListEditor.GridView();
                View.Editor.SelectionChanged += Editor_SelectionChanged;
                _toolTipController = new ToolTipController { ShowShadow = true, ShowBeak = true, AllowHtmlText = true };
            }
        }

        protected override void OnDeactivated() {
            _gridView = null;
            if (_toolTipController != null)
                _toolTipController.Dispose();
            _toolTipController = null;
            View.Editor.SelectionChanged -= Editor_SelectionChanged;
            base.OnDeactivated();
        }

        private void Editor_SelectionChanged(object sender, EventArgs e) {
            if (View.SelectedObjects.Count > 1 && _gridView != null && SelectedItemSumEnabled()) {
                var isUnboundColumn = !string.IsNullOrEmpty(_gridView.FocusedColumn.UnboundExpression);
                var columnName = isUnboundColumn ? _gridView.FocusedColumn.Caption : _gridView.FocusedColumn.FieldName;
                if (!columnName.Contains("!")) {
                    var columnType = GetColumnType();
                    switch (columnType.FullName) {
                        case "System.Int32":
                        case "System.Decimal":
                        case "System.Int64":
                        case "System.Single":
                        case "System.Double":
                            var info = Calculate(isUnboundColumn, columnName);
                            string columnMask = GetColumnMask();
                            _toolTipController.ShowHint(string.Format(columnMask, info.Total, columnName, info.MinTotal, info.Count, info.MaxTotal));
                            if (!_gridView.OptionsView.ShowFooter) {
                                _gridView.OptionsView.ShowFooter = true;
                                View.Model.IsFooterVisible = true;
                                View.SaveModel();
                            }
                            break;
                    }
                }
            }
        }

        private bool SelectedItemSumEnabled(){
            if (_gridView.FocusedColumn != null){
                var modelColumnSelectedItemSum = (IModelColumnSelectedItemSum)_gridView.FocusedColumn.Model();
                return modelColumnSelectedItemSum != null && modelColumnSelectedItemSum.SelectedItemSum;
            }
            return false;
        }

        private Type GetColumnType() {
            var columnType = _gridView.FocusedColumn.ColumnType;
            return columnType.IsGenericType ? Nullable.GetUnderlyingType(columnType) : columnType;
        }

        private string GetColumnMask() {
            string mask = _gridView.FocusedColumn.DisplayFormat.FormatString;
            mask = mask.Length > 3 ? mask.Substring(3, mask.Length - 4) : "";
            var localizedText = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "SelectedItemSumFormatString");
            return string.Format(localizedText, mask);
        }

        private CalcInfo Calculate(bool isUnboundColumn, string columnName) {
            var calcInfo = new CalcInfo();
            foreach (XPBaseObject selectedObject in View.SelectedObjects) {
                var tempvalue = GetTempvalue(isUnboundColumn, selectedObject, columnName);
                calcInfo.Total += tempvalue;
                calcInfo.Count += 1;
                if (calcInfo.Count == 1) {
                    calcInfo.MaxTotal = tempvalue;
                    calcInfo.MinTotal = tempvalue;
                }
                else {
                    if (calcInfo.MaxTotal < tempvalue)
                        calcInfo.MaxTotal = tempvalue;
                    if (calcInfo.MinTotal > tempvalue)
                        calcInfo.MinTotal = tempvalue;
                }
            }
            return calcInfo;
        }

        private decimal GetTempvalue(bool isUnboundColumn, XPBaseObject selectedObject, string columnName) {
            return isUnboundColumn
                ? +Convert.ToDecimal(selectedObject.Evaluate(_gridView.FocusedColumn.UnboundExpression))
                : (columnName.Contains(".")
                    ? +Convert.ToDecimal(selectedObject.Evaluate(CriteriaOperator.Parse(columnName)))
                    : +Convert.ToDecimal(selectedObject.GetMemberValue(columnName)));
        }
        class CalcInfo {
            public decimal Total { get; set; }

            public int Count { get; set; }

            public decimal MaxTotal { get; set; }

            public decimal MinTotal { get; set; }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelMember,IModelMemberSeletectItemSum>();
            extenders.Add<IModelColumn,IModelColumnSelectedItemSum>();
        }
    }
}