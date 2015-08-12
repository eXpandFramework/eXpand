using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelDifferenceTester.Module.FunctionalTests.RoleModel {
    [DefaultClassOptions]
    public class RoleModelObject:BaseObject {
        public RoleModelObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
