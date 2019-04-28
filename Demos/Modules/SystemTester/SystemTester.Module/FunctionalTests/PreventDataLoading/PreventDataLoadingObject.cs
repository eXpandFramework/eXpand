using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace SystemTester.Module.FunctionalTests.PreventDataLoading {
    [XpandNavigationItem(PreventDataLoadingGroupName + "/Default", "PreventDataLoadingObject_ListView")]
    
    [CloneModelView(CloneViewType.ListView, "PreventDataLoading_FiltersNotEmpty")]
    [XpandNavigationItem(PreventDataLoadingGroupName + "/FiltersNotEmpty", "PreventDataLoading_FiltersNotEmpty")]

    [CloneModelView(CloneViewType.ListView, "PreventDataLoading_FilterNotEmpty")]
    [XpandNavigationItem(PreventDataLoadingGroupName + "/FilterNotEmpty", "PreventDataLoading_FilterNotEmpty")]

    [CloneModelView(CloneViewType.ListView, "PreventDataLoading_FilterByText")]
    [XpandNavigationItem(PreventDataLoadingGroupName+"/FilterByText", "PreventDataLoading_FilterByText")]
    public class PreventDataLoadingObject:BaseObject {
        public const string PreventDataLoadingGroupName="PreventDataLoading";
        public PreventDataLoadingObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
