using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using SecurityDemo.Module;

namespace DCSecurityDemo.Module.BusinessObjects {
    [Hint(Hints.ObjectLevelSecurityObject, ViewType.Any, "Description")]
    [NavigationItem(NavigationGroups.ObjectLevelSecurity)]
    [ImageName("Demo_Security_ObjectLevel")]
    [ModelDefault("Caption", "Instance-Level Protected Object")]
    [DomainComponent]
    public interface IObjectLevelSecurityObject : ISecurityDemoBaseObject {
        string Description { get; set; }
    }
}
