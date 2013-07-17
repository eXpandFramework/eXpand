using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.SystemModule.Search {
    public class SearchFromListViewController : SearchFromViewController {
        protected override void OnActivated() {
            base.OnActivated();
            var filterController = Frame.GetController<FilterController>();
            var modelListView = View.Model as IModelListViewFullTextSearch;
            if (modelListView != null && modelListView.FullTextSearchMode.HasValue) {
                filterController.FullTextSearchMode = modelListView.FullTextSearchMode.Value;
            }
            filterController.CustomGetFullTextSearchProperties += OnCustomGetFullTextSearchProperties;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<FilterController>().CustomGetFullTextSearchProperties -= OnCustomGetFullTextSearchProperties;
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