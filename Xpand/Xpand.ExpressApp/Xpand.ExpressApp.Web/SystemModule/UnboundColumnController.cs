﻿using System;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class UnboundColumnController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            if (GridListEditor != null)
                GridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs createCustomModelSynchronizerEventArgs) {
            CustomModelSynchronizerHelper.Assign(createCustomModelSynchronizerEventArgs, new UnboundColumnSynchronizer(GridListEditor, View.Model));
        }

        public ASPxGridListEditor GridListEditor {
            get { return View.Editor as ASPxGridListEditor; }
        }
    }

    public class UnboundColumnSynchronizer : ModelSynchronizer<ASPxGridListEditor, IModelListView> {
        public UnboundColumnSynchronizer(ASPxGridListEditor control, IModelListView model)
            : base(control, model) {
        }

        void ApplyModelCore(GridViewDataColumn columnWithInfo) {
            var modelColumn = (IModelColumnUnbound)columnWithInfo.Model(Model);
            columnWithInfo.FieldName = modelColumn.Id;
            columnWithInfo.UnboundType = UnboundColumnType.Object;
            columnWithInfo.UnboundExpression = modelColumn.UnboundExpression;
        }

        protected override void ApplyModelCore() {
            ForEachColumnLink(ApplyModelCore);
        }

        public override void SynchronizeModel() {
            ForEachColumnLink(SynchronizeModel);
        }

        void SynchronizeModel(GridViewDataColumn columnWithInfo) {
            ((IModelColumnUnbound)columnWithInfo.Model(Model)).UnboundExpression = columnWithInfo.UnboundExpression;
        }

        void ForEachColumnLink(Action<GridViewDataColumn> action) {
            Model.Columns.OfType<IModelColumnUnbound>().Each(unbound => {
                var xafGridColumn = Control.Grid.Columns[unbound.PropertyName] as GridViewDataColumn;
                if (xafGridColumn != null) action.Invoke(xafGridColumn);
            });
        }

    }
}