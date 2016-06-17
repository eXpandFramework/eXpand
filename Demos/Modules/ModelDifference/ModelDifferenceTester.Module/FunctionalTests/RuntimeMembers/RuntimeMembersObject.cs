using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelDifferenceTester.Module.FunctionalTests.RuntimeMembers {
    [DefaultClassOptions]
    public class RuntimeMembersObject:BaseObject {
        public RuntimeMembersObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
