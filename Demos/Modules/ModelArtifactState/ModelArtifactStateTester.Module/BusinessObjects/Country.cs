using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace ModelArtifactStateTester.Module.BusinessObjects {
    [DefaultClassOptions]
    [NavigationItem("Conditional")]
    public class Country : XPObject {
        public Country(Session session)
            : base(session) {
        }


        // Fields...
        private bool _eUMember;
        private string _name;

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }


        public bool EUMember {
            get { return _eUMember; }
            set { SetPropertyValue("EUMember", ref _eUMember, value); }
        }
    }

}