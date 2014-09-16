using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelDifferenceTester.Module.FunctionalTests {
    [DefaultClassOptions]
    public class DifferenceTesterObject:BaseObject {
        public DifferenceTesterObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
