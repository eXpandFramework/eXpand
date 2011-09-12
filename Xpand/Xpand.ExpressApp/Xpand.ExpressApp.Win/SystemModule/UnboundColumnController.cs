using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class UnboundColumnController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            if (GridListEditor != null)
                GridListEditor.ColumnCreated += GridListEditorOnColumnCreated;
        }

        void GridListEditorOnColumnCreated(object sender, ColumnCreatedEventArgs columnCreatedEventArgs) {
            var modelColumnUnbound = (columnCreatedEventArgs.ColumnInfo as IModelColumnUnbound);
            if (modelColumnUnbound != null) {
                var column = (XafGridColumn)columnCreatedEventArgs.Column;
                column.FieldName = modelColumnUnbound.Id;
                column.UnboundType = UnboundColumnType.Object;
                column.OptionsColumn.AllowEdit = false;
                column.ShowUnboundExpressionMenu = modelColumnUnbound.ShowUnboundExpressionMenu;
                column.UnboundExpression = modelColumnUnbound.UnboundExpression;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridListEditor != null) {
                GridListEditor.GridView.ColumnUnbountExpressionChanged += GridViewOnColumnUnbountExpressionChanged;
            }
        }

        void GridViewOnColumnUnbountExpressionChanged(object sender, ColumnEventArgs columnEventArgs) {
            ForEachColumnLink(SyncFromControl);
        }

        public GridListEditor GridListEditor {
            get { return View.Editor as GridListEditor; }
        }

        void SyncFromControl(IModelColumnUnbound options, XafGridColumn column) {
            options.UnboundExpression = column.UnboundExpression;
        }


        void ForEachColumnLink(Action<IModelColumnUnbound, XafGridColumn> action) {
            View.Model.Columns.OfType<IModelColumnUnbound>().Each(unbound => action.Invoke(unbound, (XafGridColumn)GridListEditor.GridView.Columns[unbound.Id]));
        }
    }
}