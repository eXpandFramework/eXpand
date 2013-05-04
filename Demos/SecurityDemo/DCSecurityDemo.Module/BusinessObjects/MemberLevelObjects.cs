using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using SecurityDemo.Module;

namespace DCSecurityDemo.Module.BusinessObjects {
    [DomainComponent]
    public interface IMemberLevelReferencedObject1 : ISecurityDemoBaseObject {
        IMemberLevelSecurityObject Owner { get; set; }
    }

    [DomainComponent]
    public interface IMemberLevelReferencedObject2 : ISecurityDemoBaseObject {
        IMemberLevelSecurityObject Owner { get; set; }
    }

    [Hint(Hints.MemberLevelSecurityObjectHint, ViewType.Any)]
    [NavigationItem(NavigationGroups.MemberLevelSecurity)]
    [ImageName("Demo_Security_MemberLevel")]
    [ModelDefault("Caption", "Member-Level Protected Object")]
    [DomainComponent]
    public interface IMemberLevelSecurityObject : ISecurityDemoBaseObject {
        string ProtectedContentProperty { get; set; }
        string ReadOnlyProperty { get; set; }
        string ReadWriteProperty { get; set; }
        IList<IMemberLevelReferencedObject1> ProtectedContentCollection { get; }
        IList<IMemberLevelReferencedObject2> ReadOnlyCollection { get; }
    }
}
