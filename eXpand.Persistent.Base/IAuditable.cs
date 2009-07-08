using DevExpress.Xpo;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.Persistent.Base.Interfaces {
    public interface IAuditable {
        XPCollection<AuditDataItemPersistent> AuditTrail { get; }
    }
}
