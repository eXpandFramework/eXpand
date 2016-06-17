using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelDifferenceTester.Module.FunctionalTests.ApplicationModel {
    [DefaultClassOptions]
    public class ApplicationModelObject:BaseObject {
        public ApplicationModelObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
