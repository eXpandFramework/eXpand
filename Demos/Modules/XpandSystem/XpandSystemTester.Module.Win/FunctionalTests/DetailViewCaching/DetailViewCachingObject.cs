using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Win.FunctionalTests.DetailViewCaching {
    [DefaultClassOptions]
    public class DetailViewCachingObject:BaseObject {
        public DetailViewCachingObject(Session session) : base(session){
        }

        public int Year { get; set; }
    }
}
