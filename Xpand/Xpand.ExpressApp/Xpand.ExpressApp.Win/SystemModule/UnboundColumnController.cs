using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class UnboundColumnController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            if (GridListEditor != null)
                GridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs createCustomModelSynchronizerEventArgs) {
            createCustomModelSynchronizerEventArgs.ModelSynchronizer = new UnboundColumnSynchronizer(GridListEditor, View.Model);
        }

        //        protected override void OnViewControlsCreated() {
        //            base.OnViewControlsCreated();
        //            if (GridListEditor != null) {
        //                GridListEditor.GridView.ColumnUnbountExpressionChanged += GridViewOnColumnUnbountExpressionChanged;
        //            }
        //        }

        //        void GridViewOnColumnUnbountExpressionChanged(object sender, ColumnEventArgs columnEventArgs) {
        //            ForEachColumnLink(SyncFromControl);
        //        }

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

    public class UnboundColumnSynchronizer : ModelSynchronizer<GridListEditor, IModelListView> {
        public UnboundColumnSynchronizer(GridListEditor control, IModelListView model)
            : base(control, model) {
        }
        void ApplyModelCore(IModelColumnUnbound modelColumnUnbound, XafGridColumn column) {
            column.FieldName = modelColumnUnbound.Id;
            column.UnboundType = UnboundColumnType.Object;
            column.OptionsColumn.AllowEdit = false;
            column.ShowUnboundExpressionMenu = modelColumnUnbound.ShowUnboundExpressionMenu;
            column.UnboundExpression = modelColumnUnbound.UnboundExpression;

        }

        protected override void ApplyModelCore() {
            ForEachColumnLink(ApplyModelCore);
        }

        public override void SynchronizeModel() {
            ForEachColumnLink(SynchronizeModel);
        }

        void SynchronizeModel(IModelColumnUnbound modelColumnUnbound, XafGridColumn xafGridColumn) {
            modelColumnUnbound.UnboundExpression = xafGridColumn.UnboundExpression;
        }

        void ForEachColumnLink(Action<IModelColumnUnbound, XafGridColumn> action) {
            Model.Columns.OfType<IModelColumnUnbound>().Each(unbound => {
                var xafGridColumn = Control.GridView.Columns[unbound.PropertyName] as XafGridColumn;
                if (xafGridColumn != null) action.Invoke(unbound, xafGridColumn);
            });
        }

    }
}