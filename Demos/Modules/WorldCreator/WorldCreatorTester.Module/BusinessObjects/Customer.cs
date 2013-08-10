using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace WorldCreatorTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Customer : BaseObject {
        string _firstName;
        string _lastName;

        public Customer(Session session) : base(session) {
        }

        public string FirstName {
            get { return _firstName; }
            set { SetPropertyValue("FirstName", ref _firstName, value); }
        }

        public string LastName {
            get { return _lastName; }
            set { SetPropertyValue("LastName", ref _lastName, value); }
        }
    }
}