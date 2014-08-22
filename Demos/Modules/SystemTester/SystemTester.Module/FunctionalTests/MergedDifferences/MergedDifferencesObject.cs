using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.FunctionalTests.MergedDifferences {
    [DefaultClassOptions]
    public class MergedDifferencesObject:BaseObject {
        public MergedDifferencesObject(Session session) : base(session){
        }

        

        public string Name { get; set; }
    }
}
