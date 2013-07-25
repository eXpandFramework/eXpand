using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Model.RuntimeMembers;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class UnboundColumnController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            var gridListEditor = View.Editor as ColumnsListEditor;
            if (gridListEditor != null)
                gridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
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
                column.UnboundType = UnboundColumnType.Object;
                column.OptionsColumn.AllowEdit = false;
                column.ShowUnboundExpressionMenu = modelColumnUnbound.ShowUnboundExpressionMenu;
                column.UnboundExpression = modelColumnUnbound.UnboundExpression;
            }
        }

        IEnumerable<GridColumn> GetXafGridColumns() {
            IEnumerable<GridColumn> xafGridColumns =
                Model.Columns.OfType<IModelColumnUnbound>().Select(
                    unbound => Control.GridView().Columns[unbound.PropertyName]).Where(column => column != null);
            return xafGridColumns;
        }

        public override void SynchronizeModel() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var xafGridColumn in xafGridColumns) {
                ((IModelColumnUnbound)xafGridColumn.Model()).UnboundExpression = xafGridColumn.UnboundExpression;
            }
        }
    }
}