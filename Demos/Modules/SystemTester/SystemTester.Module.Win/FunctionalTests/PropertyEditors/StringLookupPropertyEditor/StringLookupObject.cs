using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Win.FunctionalTests.PropertyEditors.StringLookupPropertyEditor {
    [DefaultClassOptions]
    [NavigationItem("PropertyEditors")]
    public class StringLookupObject:BaseObject {
        public StringLookupObject(Session session) : base(session){
        }

        
        public string Phone { get; set; }
    }
}
