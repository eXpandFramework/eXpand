using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class UnboundColumnController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            if (GridListEditor != null)
                GridListEditor.ColumnCreated += GridListEditorOnColumnCreated;
        }

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

        void SyncFromModel(IModelColumnUnbound modelColumnUnbound, GridViewDataColumnWithInfo column) {
            column.UnboundType = UnboundColumnType.Object;
            //                        column.ShowUnboundExpressionMenu = modelColumnOptions.ShowUnboundExpressionMenu;
            column.UnboundExpression = modelColumnUnbound.UnboundExpression;
            column.FieldName = modelColumnUnbound.Id;
        }

        void ForEachColumnLink(Action<IModelColumnUnbound, GridViewDataColumnWithInfo> action) {
            var modelColumnUnbounds = View.Model.Columns.OfType<IModelColumnUnbound>();
            modelColumnUnbounds.Each(unbound => action.Invoke(unbound, (GridViewDataColumnWithInfo)GridListEditor.Grid.Columns[unbound.PropertyName]));
        }
    }
}