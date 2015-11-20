using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.HideNavigationOnStartup {
    [DefaultClassOptions]
    public class HideNavigationOnStartupObject:BaseObject {
        public HideNavigationOnStartupObject(Session session) : base(session){
        }

        

        public string Name { get; set; }
    }
}
