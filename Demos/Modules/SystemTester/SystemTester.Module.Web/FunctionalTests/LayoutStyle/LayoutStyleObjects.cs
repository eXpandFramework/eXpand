using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.LayoutStyle{
    [DefaultClassOptions]
    public class LayoutStyleObject : BaseObject{
        public LayoutStyleObject(Session session) : base(session){
        }

        public string Name { get; set; }

    }
}