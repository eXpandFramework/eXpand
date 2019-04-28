using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace SystemTester.Module.FunctionalTests.NavigationItems{
    [DefaultClassOptions]
    [CloneModelView(CloneViewType.ListView, "NavigationItemsObject_NavItemsDataSource_ListView")]
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