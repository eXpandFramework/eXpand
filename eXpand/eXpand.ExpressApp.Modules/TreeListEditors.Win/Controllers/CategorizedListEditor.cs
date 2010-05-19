using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.TreeListEditors.Win.Controllers {
    public class CategorizedListEditor : DevExpress.ExpressApp.TreeListEditors.Win.CategorizedListEditor {
        readonly Locker locker = new Locker();

        public CategorizedListEditor(IModelListView model) : base(model) {
        }

        protected override object CreateControlsCore() {
            object result = base.CreateControlsCore();
            CategoriesListView.SelectionChanged += CategoriesListView_SelectionChanged;
            var objectTreeList = CategoriesListView.Editor.Control as ObjectTreeList;
            if (objectTreeList != null) {
                objectTreeList.NodesReloading += objectTreeList_NodesReloading;
                objectTreeList.NodesReloaded += objectTreeList_NodesReloaded;
            }
            locker.LockedChanged += locker_LockedChanged;
            return result;
        }

        void objectTreeList_NodesReloaded(object sender, EventArgs e) {
            locker.Unlock();
        }

        void objectTreeList_NodesReloading(object sender, EventArgs e) {
            locker.Lock();
        }

        void CategoriesListView_SelectionChanged(object sender, EventArgs e) {
            if (!locker.Locked) {
                UpdateGridViewFilter();
            }
            else {
                locker.Call("UpdateGridViewFilter");
            }
        }

        void locker_LockedChanged(object sender, LockedChangedEventArgs e) {
            if (!e.Locked && e.PendingCalls.Contains("UpdateGridViewFilter")) {
                UpdateGridViewFilter();
            }
        }

        void UpdateGridViewFilter() {
            if (CategoriesListView.CurrentObject != null) {
                var categories = new List<object>();
                var currentCategory = (XPBaseObject) CategoriesListView.CurrentObject;
                categories.Add(currentCategory.ClassInfo.KeyProperty.GetValue(currentCategory));
                GetCategories((ITreeNode) CategoriesListView.CurrentObject, categories);
                ItemsDataSource.Criteria[CategoryPropertyName] = new InOperator("Category.Oid", categories);
            }
        }

        void GetCategories(ITreeNode current, IList<object> childrenCategoryIDs) {
            foreach (ITreeNode child in current.Children) {
                var categorizedItem = child as XPBaseObject;
                if (categorizedItem != null) {
                    childrenCategoryIDs.Add(categorizedItem.ClassInfo.KeyProperty.GetValue(categorizedItem));
                    GetCategories(child, childrenCategoryIDs);
                }
            }
        }

        public override void Dispose() {
            CategoriesListView.CurrentObjectChanged -= CategoriesListView_SelectionChanged;
            locker.LockedChanged -= locker_LockedChanged;
            base.Dispose();
        }
    }
}