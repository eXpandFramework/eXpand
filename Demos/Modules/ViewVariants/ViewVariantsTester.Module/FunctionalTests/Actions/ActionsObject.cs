using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ViewVariantsTester.Module.FunctionalTests.Actions {
    [DefaultClassOptions]
    public class ActionsObject:BaseObject {
        public ActionsObject(Session session) : base(session){
        }

        public string PropertyName { get; set; }
        public string PropertyName2 { get; set; }
    }
}
