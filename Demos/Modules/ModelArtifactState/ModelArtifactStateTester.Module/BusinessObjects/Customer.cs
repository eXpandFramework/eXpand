using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace ModelArtifactStateTester.Module.BusinessObjects {
    [DefaultClassOptions]
    [NavigationItem("Conditional")]
    public class Customer : XPObject {

        public Customer(Session session)
            : base(session) {
        }

        private Country _country;
        private string _name;

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        public Country Country {
            get { return _country; }
            set { SetPropertyValue("Country", ref _country, value); }
        }
    }

}