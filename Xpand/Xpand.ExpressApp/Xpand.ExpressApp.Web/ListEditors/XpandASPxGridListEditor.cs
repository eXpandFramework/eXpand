using System;
using System.Collections;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web.ListEditors {
    class MasterDetailProvider {
        ASPxGridView _gridView;
        bool _inGetFocusedObject;

        public SelectionType SelectionType {
            get { return SelectionType.Full; }
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

        public void SetupGridView(ASPxGridView gridView, Action onFocusedObjectChanged) {
            _gridView = gridView;
            gridView.SettingsBehavior.AllowFocusedRow = true;
            gridView.Load += (s, e) => onFocusedObjectChanged();

            gridView.ClientSideEvents.Init = "function (s,e) { s.firstRowChangedAfterInit = true;}";
            gridView.ClientSideEvents.FocusedRowChanged = string.Format(
                @"function(s,e) {{ 
                    {0}
                    var parentSplitter = XpandHelper.GetParentControl(s);
                    var up = XpandHelper.GetFirstChildControl(parentSplitter.GetPane(1).GetElement().childNodes[0]);
                    if ((s.firstRowChangedAfterInit!==true || !XpandHelper.IsRootSplitter(parentSplitter)) && up && up.GetMainElement()) {{ 
                        up.PerformCallback(s.GetFocusedRowIndex());}} 
                    s.firstRowChangedAfterInit = false; }}", XpandLayoutManager.GetXpandHelperScript());

            gridView.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
        }

        public IList GetSelectedObjects(object focusedObject) {
            return focusedObject != null ? new[] { focusedObject } : new object[0];
        }
    }
    [ListEditor(typeof(object))]
    public class XpandASPxGridListEditor : ASPxGridListEditor, IXpandListEditor {
        object _lastFiredFocusedObject;
        readonly MasterDetailProvider _masterDetailProvider = new MasterDetailProvider();

        public event EventHandler<ViewControlCreatedEventArgs> ViewControlsCreated;
        public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;
        public event EventHandler<CustomCreateWebDataSourceEventArgs> CustomCreateWebDataSource;

        public XpandASPxGridListEditor(IModelListView info)
            : base(info) {
        }

        protected override WebDataSource CreateWebDataSource(object collection){
            var args = new CustomCreateWebDataSourceEventArgs(collection);
            OnCustomCreateWebDataSource(args);
            if (args.Handled)
                collection = args.Collection;
            return base.CreateWebDataSource(collection);
        }

        protected virtual void OnCustomCreateWebDataSource(CustomCreateWebDataSourceEventArgs e){
            var handler = CustomCreateWebDataSource;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnColumnCreated(ColumnCreatedEventArgs e) {
            EventHandler<ColumnCreatedEventArgs> handler = ColumnCreated;
            if (handler != null) handler(this, e);
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
            IList selectedObjects = base.GetSelectedObjects();
            if (!MasterDetail || (selectedObjects != null && selectedObjects.Count > 0))
                return selectedObjects;
            return _masterDetailProvider.GetSelectedObjects(FocusedObject);
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
            if (Grid != null && MasterDetail) SetFirstRowChangeAfterInit(Grid, false);
            if (Grid != null) Grid.FocusedRowIndex = -1;
            OnFocusedObjectChanged();
        }

        private void SetFirstRowChangeAfterInit(ASPxGridView grid, bool value) {
            grid.ClientSideEvents.Init = string.Format(CultureInfo.InvariantCulture,
                "function (s,e) {{ s.firstRowChangedAfterInit = {0};}}", value ? "true" : "false");
        }

        protected override ASPxGridView CreateGridControl() {
            ASPxGridView gridView = base.CreateGridControl();
            if (MasterDetail)
                _masterDetailProvider.SetupGridView(gridView, OnFocusedObjectChanged);
            return gridView;
        }

        protected override GridViewDataColumn CreateColumn(IModelColumn columnInfo) {
            GridViewDataColumn gridViewDataColumnWithInfo = base.CreateColumn(columnInfo);
            OnColumnCreated(new ColumnCreatedEventArgs(gridViewDataColumnWithInfo));
            return gridViewDataColumnWithInfo;
        }

        public override void SetControlSelectedObjects(IList<object> objects) {
            if (!MasterDetail || objects.Count != 1) {
                base.SetControlSelectedObjects(objects);
            } else {
                Grid.Selection.UnselectAll();
                Grid.FocusedRowIndex = Grid.FindVisibleIndexByKeyValue(((WebDataSource)Grid.DataSource).View.GetKeyValue(objects[0]));
                OnSelectionChanged();
            }
        }

        public void NotifyViewControlsCreated(XpandListView listView) {
            if (listView == null)
                throw new ArgumentNullException("listView");

            if (ViewControlsCreated != null)
                ViewControlsCreated(this, new ViewControlCreatedEventArgs(listView.IsRoot));

        }

    }

    public class CustomCreateWebDataSourceEventArgs : HandledEventArgs{
        public CustomCreateWebDataSourceEventArgs(object collection){
            Collection = collection;
        }

        public object Collection { get; set; }

    }


    public class ColumnCreatedEventArgs : EventArgs {
        private readonly GridViewDataColumn _gridViewDataColumnWithInfo;

        public ColumnCreatedEventArgs(GridViewDataColumn gridViewDataColumnWithInfo) {
            _gridViewDataColumnWithInfo = gridViewDataColumnWithInfo;
        }

        public GridViewDataColumn GridViewDataColumnWithInfo {
            get { return _gridViewDataColumnWithInfo; }
        }
    }

    
}
