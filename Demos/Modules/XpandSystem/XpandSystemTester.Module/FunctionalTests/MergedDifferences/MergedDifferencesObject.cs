using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.FunctionalTests.MergedDifferences {
    [DefaultClassOptions]
    [DefaultProperty("Url")]
    public class MergedDifferencesObject:BaseObject {
        public MergedDifferencesObject(Session session) : base(session){
        }

        

        public string Name { get; set; }
    }
}
