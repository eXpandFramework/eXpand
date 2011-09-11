using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class ColumnUnboundExpressionController : ViewController<ListView> {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridListEditor != null) {
                ForEachColumnLink(SyncFromModel);
                GridListEditor.GridView.ColumnUnbountExpressionChanged += GridViewOnColumnUnbountExpressionChanged;
            }
        }

        void GridViewOnColumnUnbountExpressionChanged(object sender, ColumnEventArgs columnEventArgs) {
            ForEachColumnLink(SyncFromControl);
        }

        public GridListEditor GridListEditor {
            get { return View.Editor as GridListEditor; }
        }

        void SyncFromControl(IModelGridColumnOptions options, GridColumn column) {
            options.UnboundExpression = column.UnboundExpression;
        }

        void SyncFromModel(IModelGridColumnOptions modelColumnOptions, GridColumn column) {
            column.UnboundType = UnboundColumnType.Object;
            column.OptionsColumn.AllowEdit = false;
            column.ShowUnboundExpressionMenu = modelColumnOptions.ShowUnboundExpressionMenu;
            column.UnboundExpression = modelColumnOptions.UnboundExpression;
        }

        void ForEachColumnLink(Action<IModelGridColumnOptions, GridColumn> action) {
            var modelColumnUnbounds = View.Model.Columns.OfType<IModelColumnUnbound>();
            modelColumnUnbounds.Each(unbound => action.Invoke(((IModelColumnOptions)unbound).GridColumnOptions,
                                                                  GridListEditor.GridView.Columns[unbound.PropertyName]));
        }
    }
}