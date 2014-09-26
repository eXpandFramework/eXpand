using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace SystemTester.Module.FunctionalTests.ViewEditMode{
    [CloneView(CloneViewType.DetailView, "ViewEditModeView_DetailView")]
    [CloneView(CloneViewType.ListView, "ViewEditModeView_ListView", DetailView = "ViewEditModeView_DetailView")]
    [XpandNavigationItem("ViewEditMode/View", "ViewEditModeView_ListView")]
    [CloneView(CloneViewType.DetailView, "ViewEditModeEdit_DetailView")]
    [CloneView(CloneViewType.ListView, "ViewEditModeEdit_ListView", DetailView = "ViewEditModeEdit_DetailView")]
    [XpandNavigationItem("ViewEditMode/Edit", "ViewEditModeEdit_ListView")]
    public class ViewEditModeObject : BaseObject{
        public ViewEditModeObject(Session session) : base(session){
        }

        public string Name { get; set; }

        [Association("ViewEditModeObject-ViewEditModeChildren")]
        public XPCollection<ViewEditModeChildObject> ViewEditModeChildren{
            get { return GetCollection<ViewEditModeChildObject>("ViewEditModeChildren"); }
        }
    }

    public class ViewEditModeChildObject : BaseObject{
        private ViewEditModeObject _viewEditModeObject;

        public ViewEditModeChildObject(Session session) : base(session){
        }


        public string ChildName { get; set; }

        [Association("ViewEditModeObject-ViewEditModeChildren")]
        public ViewEditModeObject ViewEditModeObject{
            get { return _viewEditModeObject; }
            set { SetPropertyValue("ViewEditModeObject", ref _viewEditModeObject, value); }
        }
    }
}