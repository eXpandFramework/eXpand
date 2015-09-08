using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.WarnForUnsavedChanges{
    [DefaultClassOptions]
    public class WarnForUnsavedChangesObject : BaseObject{
        private string _name;

        public WarnForUnsavedChangesObject(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}