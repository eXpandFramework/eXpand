using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SecurityDemo.Module {
    public abstract class ClassLevelBaseObject : SecurityDemoBaseObject {
        private string description;
        public ClassLevelBaseObject(Session session)
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


    [Hint(Hints.FullAccessObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_FullAccess")]
    [ModelDefault("Caption", "Fully Accessible Object")]
    public class FullAccessObject : ClassLevelBaseObject {
        public FullAccessObject(Session session)
            : base(session) {
        }
    }

    [Hint(Hints.ProtectedContentObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_ProtectedContent")]
    [ModelDefault("Caption", "Protected Object")]
    public class ProtectedContentObject : ClassLevelBaseObject {
        public ProtectedContentObject(Session session)
            : base(session) {
        }
    }

    [Hint(Hints.ReadOnlyObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_ReadOnly")]
    [ModelDefault("Caption", "Read-Only Object")]
    public class ReadOnlyObject : ClassLevelBaseObject {
        public ReadOnlyObject(Session session)
            : base(session) {
        }
    }

    [Hint(Hints.IrremovableObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_Irremovable")]
    [ModelDefault("Caption", "Protected Deletion Object")]
    public class IrremovableObject : ClassLevelBaseObject {
        public IrremovableObject(Session session)
            : base(session) {
        }
    }

    [Hint(Hints.UncreatableObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_Uncreatable")]
    [ModelDefault("Caption", "Protected Creation Object")]
    public class UncreatableObject : ClassLevelBaseObject {
        public UncreatableObject(Session session)
            : base(session) {
        }
    }
}
