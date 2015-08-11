using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelDifferenceTester.Module.FunctionalTests.UserModel {
    [DefaultClassOptions]
    public class UserModelObject:BaseObject {
        public UserModelObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
