using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;

namespace XtraDashboardTester.Module.BusinessObjects{
    [DefaultClassOptions]
    public class Customer : Person{
        private PermissionPolicyUser _user;

        public Customer(Session session) : base(session){
        }

        string _test;

        public string Test{
            get{ return _test; }
            set{ SetPropertyValue(nameof(Test), ref _test, value); }
        }
        public PermissionPolicyUser User{
            get { return _user; }
            set { SetPropertyValue("User", ref _user, value); }
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            if (SecuritySystem.CurrentUser != null)
                User = Session.GetObjectByKey<PermissionPolicyUser>(Session.GetKeyValue(SecuritySystem.CurrentUser));
        }
    }
}