using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SecurityTester.Module.Win.FunctionalTests.OverallCustomizationAllowed {
    [DefaultClassOptions]
    public class OverallCustomizationAllowedObject:BaseObject {
        public OverallCustomizationAllowedObject(Session session) : base(session){
        }

        public string Field { get; set; }
    }
}
