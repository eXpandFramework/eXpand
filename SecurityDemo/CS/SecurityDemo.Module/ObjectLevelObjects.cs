using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp;

namespace SecurityDemo.Module {
    [Hint(Hints.ObjectLevelSecurityObject, ViewType.Any, "Description")]
    [NavigationItem(NavigationGroups.ObjectLevelSecurity)]
    [ImageName("Demo_Security_ObjectLevel")]
    [Custom("Caption", "Instance-Level Protected Object")]
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
