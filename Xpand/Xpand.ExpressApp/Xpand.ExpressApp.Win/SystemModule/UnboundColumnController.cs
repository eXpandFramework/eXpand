using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class UnboundColumnController : ExpressApp.Model.UnboundColumnController {
        protected override void OnActivated() {
            base.OnActivated();
            var gridListEditor = View.Editor as ColumnsListEditor;
            if (gridListEditor != null)
                gridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
        }

        protected override void AddColumn(IModelColumnUnbound modelColumnUnbound) {
            var columnWrapper = ((GridListEditor) View.Editor).AddColumn(modelColumnUnbound);
            columnWrapper.VisibleIndex = 0;
            new UnboundColumnSynchronizer((ColumnsListEditor) View.Editor, View.Model).ApplyModel();
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs createCustomModelSynchronizerEventArgs) {
            CustomModelSynchronizerHelper.Assign(createCustomModelSynchronizerEventArgs, new UnboundColumnSynchronizer((ColumnsListEditor)sender, View.Model));
        }
    }

    public class UnboundColumnSynchronizer : ModelSynchronizer<ColumnsListEditor, IModelListView> {
        public UnboundColumnSynchronizer(ColumnsListEditor control, IModelListView model)
            : base(control, model) {
        }

        protected override void ApplyModelCore() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var column in xafGridColumns) {
                var modelColumnUnbound = (IModelColumnUnbound)column.Model();
                column.FieldName = modelColumnUnbound.Id;
                column.UnboundType = (UnboundColumnType) Enum.Parse(typeof(UnboundColumnType),modelColumnUnbound.UnboundType.ToString());
                column.OptionsColumn.AllowEdit = false;
                column.ShowUnboundExpressionMenu = modelColumnUnbound.ShowUnboundExpressionMenu;
                column.UnboundExpression = modelColumnUnbound.UnboundExpression;
            }
        }

        IEnumerable<GridColumn> GetXafGridColumns() {
            return Model.Columns.OfType<IModelColumnUnbound>().Select(
                    unbound => Control.GridView().Columns[unbound.PropertyName]).Where(column => column != null);
        }

        public override void SynchronizeModel() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var xafGridColumn in xafGridColumns) {
                ((IModelColumnUnbound)xafGridColumn.Model()).UnboundExpression = xafGridColumn.UnboundExpression;
            }
        }
    }
}