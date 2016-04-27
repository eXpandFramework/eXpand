using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace SystemTester.Module.FunctionalTests.NavigationItems{
    [DefaultClassOptions]
    [CloneView(CloneViewType.ListView, "NavigationItemsObject_NavItemsDataSource_ListView")]
    [XpandNavigationItem("NavigationItems/Navigation Items Object")]
    public class NavigationItemsObject : BaseObject{
        public NavigationItemsObject(Session session) : base(session){
        }

        private string _name;

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}