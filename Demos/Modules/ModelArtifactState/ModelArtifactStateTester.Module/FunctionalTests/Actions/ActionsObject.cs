using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelArtifactStateTester.Module.FunctionalTests.Actions {
    [DefaultClassOptions]
    public class ActionsObject:BaseObject {
        public ActionsObject(Session session) : base(session){
        }
    }
}
