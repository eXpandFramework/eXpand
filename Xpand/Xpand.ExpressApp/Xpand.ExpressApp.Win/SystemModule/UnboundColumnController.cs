using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class UnboundColumnController : ExpressApp.Model.UnboundColumnController {
        protected override void OnDeactivated(){
            var gridListEditor = View.Editor as WinColumnsListEditor;
            if (gridListEditor != null) {
                gridListEditor.ModelSaved -= GridListEditorOnModelSaved;
                gridListEditor.ModelApplied -= GridListEditorOnModelApplied;
            }
            base.OnDeactivated();
        }

        protected override void OnActivated() {
            base.OnActivated();
            var gridListEditor = View.Editor as WinColumnsListEditor;
            if (gridListEditor != null){
                gridListEditor.ModelSaved+=GridListEditorOnModelSaved;
                gridListEditor.ModelApplied+=GridListEditorOnModelApplied;
            }
        }

        protected override void AddColumn(IModelColumnUnbound modelColumnUnbound) {
            var columnWrapper = ((GridListEditor)View.Editor).AddColumn(modelColumnUnbound);
            columnWrapper.VisibleIndex = 0;
            new UnboundColumnSynchronizer((WinColumnsListEditor)View.Editor, View.Model).ApplyModel();
        }

        private void GridListEditorOnModelApplied(object sender, EventArgs eventArgs){
            new UnboundColumnSynchronizer((WinColumnsListEditor)sender, View.Model).ApplyModel();
        }

        private void GridListEditorOnModelSaved(object sender, EventArgs eventArgs){
            new UnboundColumnSynchronizer((WinColumnsListEditor)sender, View.Model).SynchronizeModel();
        }
    }

    public class UnboundColumnSynchronizer : ModelSynchronizer<WinColumnsListEditor, IModelListView> {
        public UnboundColumnSynchronizer(WinColumnsListEditor control, IModelListView model)
            : base(control, model) {
        }

        protected override void ApplyModelCore() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var column in xafGridColumns) {
                var modelColumnUnbound = (IModelColumnUnbound)column.Model();
                column.FieldName = Guid.NewGuid().ToString();
                column.Caption = modelColumnUnbound.Caption;
                column.UnboundType = (UnboundColumnType)Enum.Parse(typeof(UnboundColumnType), modelColumnUnbound.UnboundType.ToString());
                column.OptionsColumn.AllowEdit = false;
                column.ShowUnboundExpressionMenu = modelColumnUnbound.ShowUnboundExpressionMenu;
                column.UnboundExpression = modelColumnUnbound.UnboundExpression;
            }
        }

        IEnumerable<GridColumn> GetXafGridColumns() {
            var gridView = Control.GridView();
            if (gridView == null)
                return Enumerable.Empty<GridColumn>();
            var modelColumnUnbounds = Model.Columns.OfType<IModelColumnUnbound>();
            return gridView.Columns
                        .Select(column => new { column, columnView = column.GetModel() })
                        .Select(@t => new { @t, modelColumnUnbound = @t.columnView as IModelColumnUnbound })
                        .Where(@t => @t.modelColumnUnbound != null && modelColumnUnbounds.Contains(@t.modelColumnUnbound))
                        .Select(@t => @t.@t.column);

        }

        public override void SynchronizeModel() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var xafGridColumn in xafGridColumns) {
                var modelColumnUnbound = ((IModelColumnUnbound)xafGridColumn.Model());
                modelColumnUnbound.UnboundExpression = xafGridColumn.UnboundExpression;
                modelColumnUnbound.ShowUnboundExpressionMenu = xafGridColumn.ShowUnboundExpressionMenu;
            }
        }
    }
}