using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.ConditionalAppearence {
    [DefaultClassOptions]
    public class ConditionalAppearenceObject:BaseObject {
        public ConditionalAppearenceObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
