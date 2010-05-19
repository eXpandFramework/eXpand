using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelViewPropertySearchMode : IModelNode
    {
        [Category("eXpand")]
        SearchMemberMode SearchMemberMode { get; set; }
    }
    public class SearchFromListViewController : SearchFromViewController
    {

        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<DevExpress.ExpressApp.SystemModule.FilterController>().CustomGetFullTextSearchProperties += OnCustomGetFullTextSearchProperties;

        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<DevExpress.ExpressApp.SystemModule.FilterController>().CustomGetFullTextSearchProperties += OnCustomGetFullTextSearchProperties;
        }
        void OnCustomGetFullTextSearchProperties(object sender, CustomGetFullTextSearchPropertiesEventArgs customGetFullTextSearchPropertiesEventArgs)
        {
            var filterController = ((DevExpress.ExpressApp.SystemModule.FilterController)sender);
            var fullTextSearchProperties = new List<string>(GetFullTextSearchProperties(filterController.FullTextSearchTargetPropertiesMode));
            GetProperties(SearchMemberMode.Exclude, s => fullTextSearchProperties.Remove(s));
            GetProperties(SearchMemberMode.Include, fullTextSearchProperties.Add);
            foreach (var fullTextSearchProperty in fullTextSearchProperties)
            {
                customGetFullTextSearchPropertiesEventArgs.Properties.Add(fullTextSearchProperty);
            }
            customGetFullTextSearchPropertiesEventArgs.Handled = true;
        }

        string[] GetShownProperties()
        {
            var listView = ((ListView)View);
            if (listView.Editor != null)
            {
                return listView.Editor.ShownProperties;
            }
            return (from column in listView.Model.Columns.VisibleColumns where column.Index != -1 select column.PropertyName).ToArray();
        }

        public IEnumerable<string> GetFullTextSearchProperties(FullTextSearchTargetPropertiesMode fullTextSearchTargetPropertiesMode)
        {
            var criteriaBuilder = new SearchCriteriaBuilder(View.ObjectTypeInfo) { IncludeNonPersistentMembers = false };
            switch (fullTextSearchTargetPropertiesMode)
            {
                case FullTextSearchTargetPropertiesMode.AllSearchableMembers:
                    criteriaBuilder.FillSearchProperties();
                    criteriaBuilder.AddSearchProperties(GetShownProperties());
                    break;
                case FullTextSearchTargetPropertiesMode.VisibleColumns:
                    var shownProperties = new List<string>(GetShownProperties());
                    string friendlyKeyMemberName = FriendlyKeyPropertyAttribute.FindFriendlyKeyMemberName(View.ObjectTypeInfo, true);
                    if (!string.IsNullOrEmpty(friendlyKeyMemberName) && !shownProperties.Contains(friendlyKeyMemberName))
                    {
                        shownProperties.Add(friendlyKeyMemberName);
                    }
                    criteriaBuilder.SetSearchProperties(shownProperties);
                    break;
                default:
                    throw new ArgumentException(fullTextSearchTargetPropertiesMode.ToString(), "fullTextSearchTargetPropertiesMode");
            }
            return criteriaBuilder.SearchProperties;
        }



        void GetProperties(SearchMemberMode searchMemberMode, Action<string> action)
        {
            var listView = ((ListView)View);
            var enumerable = listView.Model.Columns.VisibleColumns.OfType<IModelViewPropertySearchMode>().Where(
                wrapper => wrapper.SearchMemberMode == searchMemberMode).Select(nodeWrapper => ((IModelColumn)nodeWrapper).PropertyName);
            foreach (var s in enumerable)
            {
                action.Invoke(s);
            }
        }

    }
}