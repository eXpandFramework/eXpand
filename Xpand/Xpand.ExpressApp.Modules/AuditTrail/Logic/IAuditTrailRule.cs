using System.ComponentModel;
using DevExpress.Persistent.AuditTrail;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public interface IAuditTrailRule:ILogicRule {
        [Category("AuditTrail")]
        bool IncludeRelatedTypes { get; set; }
        [Category("AuditTrail")]
        ObjectAuditingMode? AuditingMode { get; set; }
        [Category("AuditTrail")]
        bool AuditAllMembers { get; set; }
        
    }

}