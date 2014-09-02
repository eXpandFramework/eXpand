using System.ComponentModel;
using DevExpress.Persistent.AuditTrail;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public enum ObjectAuditingMode {None=-1, Full, Lightweight, CreationOnly };
    public interface IAuditTrailRule:ILogicRule {
        [Category("AuditTrail")]
        bool? AuditPending { get; set; }
        [Category("AuditTrail")]
        bool IncludeRelatedTypes { get; set; }
        [Category("AuditTrail")]
        ObjectAuditingMode? AuditingMode { get; set; }
        [Category("AuditTrail")]
        AuditMemberStrategy AuditMemberStrategy { get; set; }
        [Category("AuditTrail")]
        AuditTrailStrategy AuditTrailStrategy { get; set; }
    }

}