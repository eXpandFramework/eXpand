using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class UnboundColumnController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            var gridListEditor = View.Editor as GridListEditor;
            if (gridListEditor != null)
                gridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs createCustomModelSynchronizerEventArgs) {
            createCustomModelSynchronizerEventArgs.ModelSynchronizer = new UnboundColumnSynchronizer((GridListEditor)sender, View.Model);
        }
    }

    public class UnboundColumnSynchronizer : ModelSynchronizer<GridListEditor, IModelListView> {
        public UnboundColumnSynchronizer(GridListEditor control, IModelListView model)
            : base(control, model) {
        }

        protected override void ApplyModelCore() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var column in xafGridColumns) {
                var modelColumnUnbound = (IModelColumnUnbound)column.Model;
                column.FieldName = modelColumnUnbound.Id;
                column.UnboundType = UnboundColumnType.Object;
                column.OptionsColumn.AllowEdit = false;
                column.ShowUnboundExpressionMenu = modelColumnUnbound.ShowUnboundExpressionMenu;
                column.UnboundExpression = modelColumnUnbound.UnboundExpression;
            }
        }

        IEnumerable<XafGridColumn> GetXafGridColumns() {
            IEnumerable<XafGridColumn> xafGridColumns =
                Model.Columns.OfType<IModelColumnUnbound>().Select(
                    unbound => Control.GridView.Columns[unbound.PropertyName] as XafGridColumn).Where(column => column != null);
            return xafGridColumns;
        }

        public override void SynchronizeModel() {
            var xafGridColumns = GetXafGridColumns();
            foreach (var xafGridColumn in xafGridColumns) {
                ((IModelColumnUnbound)xafGridColumn.Model).UnboundExpression = xafGridColumn.UnboundExpression;
            }
        }
    }
}