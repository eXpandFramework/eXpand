using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace SystemTester.Module.FunctionalTests.PreventDataLoading {
    [XpandNavigationItem(PreventDataLoadingGroupName + "/Default", "PreventDataLoadingObject_ListView")]
    
    [CloneView(CloneViewType.ListView, "PreventDataLoading_FiltersNotEmpty")]
    [XpandNavigationItem(PreventDataLoadingGroupName + "/FiltersNotEmpty", "PreventDataLoading_FiltersNotEmpty")]

    [CloneView(CloneViewType.ListView, "PreventDataLoading_FilterNotEmpty")]
    [XpandNavigationItem(PreventDataLoadingGroupName + "/FilterNotEmpty", "PreventDataLoading_FilterNotEmpty")]

    [CloneView(CloneViewType.ListView, "PreventDataLoading_FilterByText")]
    [XpandNavigationItem(PreventDataLoadingGroupName+"/FilterByText", "PreventDataLoading_FilterByText")]
    public class PreventDataLoadingObject:BaseObject {
        public const string PreventDataLoadingGroupName="PreventDataLoading";
        public PreventDataLoadingObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
