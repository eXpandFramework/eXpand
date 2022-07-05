using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule.Search {
    public class SearchFromListViewController : SearchFromViewController {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<FilterController>(filterController => {
                var modelListView = View.Model as IModelListViewFullTextSearch;
                if (modelListView?.FullTextSearchMode != null)
                    filterController.FullTextSearchMode = modelListView.FullTextSearchMode.Value;
                filterController.FullTextFilterAction.Executing += FullTextFilterActionOnExecuting;
                filterController.CustomGetFullTextSearchProperties += OnCustomGetFullTextSearchProperties;
            });
        }

        void FullTextFilterActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            IModelView modelView = ((ViewController) ((ActionBase) sender).Controller).View.Model;
            var modelListView = ((IModelListViewFullTextSearch)modelView).FullTextListView;
            if (modelListView!=null) {
                cancelEventArgs.Cancel = true;
                var action = ((ParametrizedAction)sender);
                var searchValue = action.Value;
                var objectSpace = Application.CreateObjectSpace(modelListView.ModelClass.TypeInfo.Type);
                var collectionSource = Application.CreateCollectionSource(objectSpace, modelListView.ModelClass.TypeInfo.Type, modelListView.Id);
                var listView = Application.CreateListView(modelListView, collectionSource, true);
                Frame.SetView(listView);
                action.Value = searchValue;
                action.DoExecute(searchValue);
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<FilterController>(filterController => {
                filterController.CustomGetFullTextSearchProperties -= OnCustomGetFullTextSearchProperties;
                filterController.FullTextFilterAction.Executing -= FullTextFilterActionOnExecuting;
            });
        }

        void OnCustomGetFullTextSearchProperties(object sender, CustomGetFullTextSearchPropertiesEventArgs customGetFullTextSearchPropertiesEventArgs) {
            ListView listView = ((FilterController) sender).View;
            var xpandSearchCriteriaBuilder = new XpandSearchCriteriaBuilder(listView.ObjectTypeInfo, listView);
            var fullTextSearchProperties = GetFullTextSearchProperties(xpandSearchCriteriaBuilder,listView);
            customGetFullTextSearchPropertiesEventArgs.Properties.Clear();
            customGetFullTextSearchPropertiesEventArgs.Properties.AddRange(fullTextSearchProperties);
            customGetFullTextSearchPropertiesEventArgs.Handled = true;
        }
    }
}