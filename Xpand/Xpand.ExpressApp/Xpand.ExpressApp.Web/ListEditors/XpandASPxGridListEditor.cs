using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.ExpressApp.Web.Editors;

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
                return _gridView.GetRow(_gridView.FocusedRowIndex);
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
                    var up = window.DetailUpdatePanelControl;
                    if (s.firstRowChangedAfterInit!==true && up && up.GetMainElement()) { 
                        up.PerformCallback(s.GetFocusedRowIndex());} 
                    s.firstRowChangedAfterInit = false; }";

            gridView.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
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
                if (MasterDetail) {
                    if (value != null)
                        Grid.FocusedRowIndex = Grid.FindVisibleIndexByKeyValue(ObjectSpace.GetKeyValue(value));
                } else
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

        public override void Setup(CollectionSourceBase collectionSource, XafApplication application) {
            base.Setup(collectionSource, application);
            CollectionSource.CriteriaApplied += CollectionSource_CriteriaApplied;
        }

        void CollectionSource_CriteriaApplied(object sender, EventArgs e) {
            if (Grid != null) SetFirstRowChangeAfterInit(Grid, false);
            if (Grid != null) Grid.FocusedRowIndex = -1;
            OnFocusedObjectChanged();
        }

        private static void SetFirstRowChangeAfterInit(ASPxGridView grid, bool value) {
            grid.ClientSideEvents.Init = string.Format(CultureInfo.InvariantCulture,
                "function (s,e) {{ s.firstRowChangedAfterInit = {0};}}", value ? "true" : "false");
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
        public override void SetControlSelectedObjects(IList<object> objects)
        {
            if (objects.Count != 1)
            {
                base.SetControlSelectedObjects(objects);
            }
            else
            {
                Grid.FocusedRowIndex =  Grid.FindVisibleIndexByKeyValue(((WebDataSource)Grid.DataSource).View.GetKeyValue(objects[0]));
                OnSelectionChanged();
            }
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
