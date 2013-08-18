using System.ComponentModel;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public interface IContextAuditTrailRule : IAuditTrailRule, IContextLogicRule {
        [Category("AuditTrail")]
        [DataSourceProperty("AuditTrailMembersContexts")]
        string AuditTrailMembersContext { get; set; }
    }
}