using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace DashboardTester.Module.FunctionalTests.MasterDetail {
    [DefaultClassOptions]
    public class MasterDetailObject:BaseObject {
        

        public MasterDetailObject(Session session) : base(session){
        }

        string _name;
        public string Name{
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }


        
    }
}
