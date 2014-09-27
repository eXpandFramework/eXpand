using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace WorldCreatorTester.Module.FunctionalTests.MergeWithExistingType {
    [XpandNavigationItem("Merge/ExistingTypeToMergeWith", "ExistingTypeToMergeWith_ListView")]
    public class ExistingTypeToMergeWith : BaseObject {
        string _firstName;
        string _lastName;

        public ExistingTypeToMergeWith(Session session)
            : base(session) {
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
