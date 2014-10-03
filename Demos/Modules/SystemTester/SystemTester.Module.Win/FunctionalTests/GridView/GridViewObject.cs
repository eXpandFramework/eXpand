using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Win.FunctionalTests.GridView {
    [XpandNavigationItem("GridView/Default","GridViewObject_ListView")]
    public class GridViewObject:BaseObject {
        public GridViewObject(Session session) : base(session){
        }
    }
}
