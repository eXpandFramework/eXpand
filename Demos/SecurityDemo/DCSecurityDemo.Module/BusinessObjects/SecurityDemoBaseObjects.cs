using DevExpress.ExpressApp.DC;

namespace DCSecurityDemo.Module.BusinessObjects {
    [NonPersistentDc]
    [DomainComponent]
    public interface ISecurityDemoBaseObject {
        string Name { get; set; }
    }
}
