using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AuditTrail.Logic;

namespace AuditTrailTester.Module.BusinessObjects {
    [DefaultClassOptions]
    [AuditTrailRule("Audit_Customer", AuditTrailMembersContext = "Customer_LastName_Age_Group",ViewType = ViewType.DetailView)]
    public class Customer : Person {
        int _age;

        public Customer(Session session)
            : base(session) {
        }

        public int Age {
            get { return _age; }
            set { SetPropertyValue("Age", ref _age, value); }
        }
    }

    [DefaultClassOptions]
    public class Employee : Person {
        public Employee(Session session) : base(session) {
        }
    }
}