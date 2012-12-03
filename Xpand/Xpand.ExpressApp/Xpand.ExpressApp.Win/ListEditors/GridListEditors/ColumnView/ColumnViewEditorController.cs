using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public class ColumnViewEditorController : ViewController {
        public ColumnViewEditorController() {
            TargetViewType = ViewType.ListView;
        }

        protected ListView ListView {
            get { return (ListView)View; }
        }

        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            ITypeInfo objType = XafTypesInfo.Instance.FindTypeInfo(e.Object.GetType());
            if (ListView != null && (objType == ListView.ObjectTypeInfo)) {
                var gridEditor = ListView.Editor as IColumnViewEditor;
                if ((gridEditor != null) && (gridEditor.Grid != null) && (gridEditor.DataSource != null) &&
                    !gridEditor.Grid.ContainsFocus) {
                    Int32 objectIndex = ListHelper.GetList(gridEditor.DataSource).IndexOf(e.Object);
                    if (objectIndex >= 0) {
                        gridEditor.ColumnView.RefreshRow(gridEditor.ColumnView.GetRowHandle(objectIndex));
                    }
                }
            }
        }

        void UpdateFilterItems() {
            var gridListEditor = ListView.Editor as IColumnViewEditor;
            if ((gridListEditor != null)
                && (gridListEditor.ColumnView != null)) {
                bool isActiveFilterEnabled = gridListEditor.ColumnView.ActiveFilterEnabled;
                foreach (GridColumn column in gridListEditor.ColumnView.Columns) {
                    if (column.FilterInfo.Value != null) {
                        var newFilterInfo = new ColumnFilterInfo(ColumnFilterType.AutoFilter,
                                                                 View.ObjectSpace.GetObject(column.FilterInfo.Value),
                                                                 column.FilterInfo.FilterCriteria, string.Empty);
                        column.View.ActiveFilter.Remove(column);
                        column.FilterInfo = newFilterInfo;
                    }
                    var newMRUFilters = new ColumnFilterInfoCollection();
                    for (int i = 0; i < column.MRUFilters.Count; i++) {
                        newMRUFilters.Add(new ColumnFilterInfo(column,
                                                               View.ObjectSpace.GetObject(column.MRUFilters[i].Value),
                                                               column.MRUFilters[i].DisplayText));
                    }
                    column.MRUFilters.Clear();
                    foreach (ColumnFilterInfo filterInfo in newMRUFilters) {
                        column.MRUFilters.Add(filterInfo);
                    }
                }
                gridListEditor.ColumnView.ActiveFilterEnabled = isActiveFilterEnabled;
            }
        }

        void ListView_EditorChanging(object sender, EventArgs e) {
            UnsubscribeToListEditorEvent();
        }

        void ListView_EditorChanged(object sender, EventArgs e) {
            SubscribeToListEditorEvent();
        }

        void gridListEditor_GridDataSourceChanging(object sender, EventArgs e) {
            RefreshGridFilterObjects();
            UpdateFilterItems();
        }

        void UnsubscribeToListEditorEvent() {
            var gridListEditor = ListView.Editor as IColumnViewEditor;
            if (gridListEditor != null) {
                gridListEditor.GridDataSourceChanging -= gridListEditor_GridDataSourceChanging;
            }
        }

        void SubscribeToListEditorEvent() {
            var gridListEditor = ListView.Editor as IColumnViewEditor;
            if (gridListEditor != null) {
                gridListEditor.GridDataSourceChanging += gridListEditor_GridDataSourceChanging;
            }
        }

        void RefreshGridFilterObjects() {
            var gridListEditor = ListView.Editor as IColumnViewEditor;
            if (gridListEditor != null) {
                var criteriaProcessor = new FilterWithObjectsProcessor(ObjectSpace);
                criteriaProcessor.Process(gridListEditor.ColumnView.ActiveFilterCriteria,
                                          FilterWithObjectsProcessorMode.ObjectToObject);
            }
        }

        protected override void OnDeactivated() {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ListView.EditorChanging -= ListView_EditorChanging;
            ListView.EditorChanged -= ListView_EditorChanged;
            UnsubscribeToListEditorEvent();
            base.OnDeactivated();
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            ListView.EditorChanging += ListView_EditorChanging;
            ListView.EditorChanged += ListView_EditorChanged;
            SubscribeToListEditorEvent();
        }
    }
}