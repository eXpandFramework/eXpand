using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Editors.Standard;
using DevExpress.Web.ASPxGridView;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class ColumnUnboundExpressionController : ViewController<ListView> {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridListEditor != null) {
                ForEachColumnLink(SyncFromModel);
                //                GridListEditor.Grid.ColumnUnbountExpressionChanged += GridViewOnColumnUnbountExpressionChanged;
            }
        }

        //        void GridViewOnColumnUnbountExpressionChanged(object sender, ColumnEventArgs columnEventArgs) {
        //            ForEachColumnLink(SyncFromControl);
        //        }

        public ASPxGridListEditor GridListEditor {
            get { return View.Editor as ASPxGridListEditor; }
        }

        //        void SyncFromControl(IModelGridColumnOptions options, GridColumn column) {
        //            options.UnboundExpression = column.UnboundExpression;
        //        }

        void SyncFromModel(IModelGridColumnOptions modelColumnOptions, GridViewColumn column) {
            //            column.UnboundType = UnboundColumnType.Object;
            //            column.OptionsColumn.AllowEdit = false;
            //            column.ShowUnboundExpressionMenu = modelColumnOptions.ShowUnboundExpressionMenu;
            //            column.UnboundExpression = modelColumnOptions.UnboundExpression;
        }

        void ForEachColumnLink(Action<IModelGridColumnOptions, GridViewColumn> action) {
            var modelColumnUnbounds = View.Model.Columns.OfType<IModelColumnUnbound>();
            modelColumnUnbounds.Each(unbound => action.Invoke(((IModelColumnOptions)unbound).GridColumnOptions,
                                                                  GridListEditor.Grid.Columns[unbound.PropertyName]));
        }
    }
}