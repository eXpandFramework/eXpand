using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using SecurityDemo.Module;

namespace DCSecurityDemo.Module.BusinessObjects {
    [NonPersistentDc]
    [DomainComponent]
    public interface IClassLevelBaseObject : ISecurityDemoBaseObject {
        string Description { get; set; }
    }

    [Hint(Hints.FullAccessObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_FullAccess")]
    [ModelDefault("Caption", "Fully Accessible Object")]
    [DomainComponent]
    public interface IFullAccessObject : IClassLevelBaseObject {
    }

    [Hint(Hints.ProtectedContentObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_ProtectedContent")]
    [ModelDefault("Caption", "Protected Object")]
    [DomainComponent]
    public interface IProtectedContentObject : IClassLevelBaseObject {
    }

    [Hint(Hints.ReadOnlyObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_ReadOnly")]
    [ModelDefault("Caption", "Read-Only Object")]
    [DomainComponent]
    public interface IReadOnlyObject : IClassLevelBaseObject {
    }

    [Hint(Hints.IrremovableObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_Irremovable")]
    [ModelDefault("Caption", "Protected Deletion Object")]
    [DomainComponent]
    public interface IIrremovableObject : IClassLevelBaseObject {
    }

    [Hint(Hints.UncreatableObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.ClassLevelSecurity)]
    [ImageName("Demo_Security_Uncreatable")]
    [ModelDefault("Caption", "Protected Creation Object")]
    [DomainComponent]
    public interface IUncreatableObject : IClassLevelBaseObject {
    }
}
