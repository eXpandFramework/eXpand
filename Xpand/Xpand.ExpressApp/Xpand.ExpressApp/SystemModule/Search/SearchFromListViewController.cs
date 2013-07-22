using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.SystemModule.Search {
    public class SearchFromListViewController : SearchFromViewController {
        protected override void OnActivated() {
            base.OnActivated();
            var filterController = Frame.GetController<FilterController>();
            var modelListView = View.Model as IModelListViewFullTextSearch;
            if (modelListView != null && modelListView.FullTextSearchMode.HasValue)
                filterController.FullTextSearchMode = modelListView.FullTextSearchMode.Value;
            filterController.FullTextFilterAction.Executing += FullTextFilterActionOnExecuting;
            filterController.CustomGetFullTextSearchProperties += OnCustomGetFullTextSearchProperties;
        }

        void FullTextFilterActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            var modelListView = ((IModelListViewFullTextSearch) View.Model).FullTextListView;
            if (modelListView!=null) {
                cancelEventArgs.Cancel = true;
                var action = ((ParametrizedAction)sender);
                var searchValue = action.Value;
                var objectSpace = Application.CreateObjectSpace();
                var collectionSource = Application.CreateCollectionSource(objectSpace, modelListView.ModelClass.TypeInfo.Type, modelListView.Id);
                var listView = Application.CreateListView(modelListView, collectionSource, true);
                Frame.SetView(listView);
                action.Value = searchValue;
                action.DoExecute(searchValue);
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var filterController = Frame.GetController<FilterController>();
            filterController.CustomGetFullTextSearchProperties -= OnCustomGetFullTextSearchProperties;
            filterController.FullTextFilterAction.Executing -= FullTextFilterActionOnExecuting;
        }

        void OnCustomGetFullTextSearchProperties(object sender, CustomGetFullTextSearchPropertiesEventArgs customGetFullTextSearchPropertiesEventArgs) {
            var xpandSearchCriteriaBuilder = new XpandSearchCriteriaBuilder(View.ObjectTypeInfo, View);
            var fullTextSearchProperties = GetFullTextSearchProperties(xpandSearchCriteriaBuilder);
            customGetFullTextSearchPropertiesEventArgs.Properties.Clear();
            customGetFullTextSearchPropertiesEventArgs.Properties.AddRange(fullTextSearchProperties);
            customGetFullTextSearchPropertiesEventArgs.Handled = true;
        }
    }
}