using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;

namespace Xpand.ExpressApp.Web.ListEditors {
    class MasterDetailProvider {
        ASPxGridView _gridView;
        bool _inGetFocusedObject;

        public SelectionType SelectionType {
            get { return SelectionType.FocusedObject | SelectionType.MultipleSelection; }
        }

        public object GetFocusedObject(CollectionSourceBase collectionSource) {
            if (_inGetFocusedObject || _gridView == null)
                return null;
            try {
                _inGetFocusedObject = true;
                if (_gridView.FocusedRowIndex == -1)
                    return null;
                return collectionSource.List[_gridView.FocusedRowIndex];
            } finally {
                _inGetFocusedObject = false;
            }

        }

        public void SetupGridView(ASPxGridView gridView, Action OnFocusedObjectChanged) {
            _gridView = gridView;
            gridView.SettingsBehavior.AllowFocusedRow = true;
            gridView.Load += (s, e) => OnFocusedObjectChanged();

            gridView.ClientSideEvents.Init = "function (s,e) { s.firstRowChangedAfterInit = true;}";
            gridView.ClientSideEvents.FocusedRowChanged =
                @"function(s,e) { 
                    var up = document.getElementById('DetailUpdatePanel');
                    if (s.firstRowChangedAfterInit!==true && up && up.ClientControl) { 
                        up.ClientControl.PerformCallback(s.GetFocusedRowIndex());} 
                    s.firstRowChangedAfterInit = false; }";

            gridView.Settings.ShowVerticalScrollBar = true;
        }

        public IList GetSelectedObjects(object focusedObject) {
            return focusedObject != null ? new[] { focusedObject } : new object[0];
        }
    }
    [ListEditor(typeof(object))]
    public class XpandASPxGridListEditor : ASPxGridListEditor {
        object _lastFiredFocusedObject;
        readonly MasterDetailProvider _masterDetailProvider = new MasterDetailProvider();
        public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;

        protected virtual void OnColumnCreated(ColumnCreatedEventArgs e) {
            EventHandler<ColumnCreatedEventArgs> handler = ColumnCreated;
            if (handler != null) handler(this, e);
        }

        public XpandASPxGridListEditor(IModelListView info)
            : base(info) {
        }


        public override object FocusedObject {
            get {
                if (MasterDetail)
                    return _masterDetailProvider.GetFocusedObject(CollectionSource);
                return base.FocusedObject;
            }
            set {
                if (MasterDetail)
                    throw new NotSupportedException();
                base.FocusedObject = value;
            }
        }

        protected override void OnFocusedObjectChanged() {
            if (MasterDetail) {
                if (_lastFiredFocusedObject != FocusedObject) {
                    base.OnFocusedObjectChanged();
                    _lastFiredFocusedObject = FocusedObject;
                }
            } else {
                base.OnFocusedObjectChanged();
            }
        }

        public override IList GetSelectedObjects() {
            return MasterDetail ? _masterDetailProvider.GetSelectedObjects(FocusedObject) : base.GetSelectedObjects();
        }

        public bool MasterDetail {
            get { return Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView; }
        }

        public override SelectionType SelectionType {
            get {
                return MasterDetail ? _masterDetailProvider.SelectionType : base.SelectionType;
            }
        }
        protected override ASPxGridView CreateGridControl() {
            ASPxGridView gridView = base.CreateGridControl();
            if (MasterDetail)
                _masterDetailProvider.SetupGridView(gridView, OnFocusedObjectChanged);
            return gridView;
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
