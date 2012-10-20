using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace SecurityDemo.Module {
    [Hint(Hints.ObjectLevelSecurityObject, ViewType.Any, "Description")]
    [NavigationItem(NavigationGroups.ObjectLevelSecurity)]
    [ImageName("Demo_Security_ObjectLevel")]
    [ModelDefault("Caption", "Instance-Level Protected Object")]
    public class ObjectLevelSecurityObject : SecurityDemoBaseObject {
        private string description;
        public ObjectLevelSecurityObject(Session session)
            : base(session) {
        }
        public string Description {
            get {
                return description;
            }
            set {
                SetPropertyValue("Description", ref description, value);
            }
        }
    }
}
