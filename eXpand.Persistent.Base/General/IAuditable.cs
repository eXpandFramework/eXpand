using DevExpress.Xpo;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.Persistent.Base.General{
    public interface IAuditable {
        XPCollection<AuditDataItemPersistent> AuditTrail { get; }
    }
}