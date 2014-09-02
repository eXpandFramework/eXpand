using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XtraDashboardTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Customer:Person {
        public Customer(Session session) : base(session) {
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            if (SecuritySystem.CurrentUser != null)
                User = Session.GetObjectByKey<SecuritySystemUser>(Session.GetKeyValue(SecuritySystem.CurrentUser));
        }

        private SecuritySystemUser _user;

        public SecuritySystemUser User {
            get {
                return _user;
            }
            set {
                SetPropertyValue("User", ref _user, value);
            }
        }
    }
}
