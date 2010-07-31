using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.SystemModule {
    public class SearchFromListViewController : SearchFromViewController {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<FilterController>().CustomGetFullTextSearchProperties +=
                OnCustomGetFullTextSearchProperties;
        }

        protected override void OnDeactivating() {
            base.OnDeactivating();
            Frame.GetController<FilterController>().CustomGetFullTextSearchProperties -=
                OnCustomGetFullTextSearchProperties;
        }

        void OnCustomGetFullTextSearchProperties(object sender,
                                                 CustomGetFullTextSearchPropertiesEventArgs
                                                     customGetFullTextSearchPropertiesEventArgs) {
            var filterController = ((FilterController) sender);
            var fullTextSearchProperties =
                new List<string>(filterController.GetFullTextSearchProperties());
            GetProperties(SearchMemberMode.Exclude, s => fullTextSearchProperties.Remove(s));
            GetProperties(SearchMemberMode.Include, fullTextSearchProperties.Add);
            foreach (string fullTextSearchProperty in fullTextSearchProperties) {
                customGetFullTextSearchPropertiesEventArgs.Properties.Add(fullTextSearchProperty);
            }
            customGetFullTextSearchPropertiesEventArgs.Handled = true;
        }




        void GetProperties(SearchMemberMode searchMemberMode, Action<string> action) {
            var listView = ((ListView) View);
            IEnumerable<string> enumerable = listView.Model.Columns.OfType
                <IModelColumnSearchMode>().Where(
                    wrapper => wrapper.SearchMemberMode == searchMemberMode).Select(
                        nodeWrapper => ((IModelColumn) nodeWrapper).PropertyName);
            foreach (string s in enumerable) {
                action.Invoke(s);
            }
        }
    }
}