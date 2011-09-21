using System;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Xpand.ExpressApp.Web.ListEditors {
    [ListEditor(typeof(object))]
    public class XpandASPxGridListEditor : ASPxGridListEditor {
        public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;

        protected virtual void OnColumnCreated(ColumnCreatedEventArgs e) {
            EventHandler<ColumnCreatedEventArgs> handler = ColumnCreated;
            if (handler != null) handler(this, e);
        }

        public XpandASPxGridListEditor(IModelListView info)
            : base(info) {
        }
        protected override GridViewDataColumnWithInfo CreateColumn(IModelColumn columnInfo) {
            GridViewDataColumnWithInfo gridViewDataColumnWithInfo = base.CreateColumn(columnInfo);
            OnColumnCreated(new ColumnCreatedEventArgs(gridViewDataColumnWithInfo));
            return gridViewDataColumnWithInfo;
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new XpandASPxGridListEditorSynchronizer(this, Model);
        }
    }

    public class ColumnCreatedEventArgs : EventArgs {
        private readonly GridViewDataColumnWithInfo _gridViewDataColumnWithInfo;

        public ColumnCreatedEventArgs(GridViewDataColumnWithInfo gridViewDataColumnWithInfo) {
            _gridViewDataColumnWithInfo = gridViewDataColumnWithInfo;
        }

        public GridViewDataColumnWithInfo GridViewDataColumnWithInfo {
            get { return _gridViewDataColumnWithInfo; }
        }
    }
}
