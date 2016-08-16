using System.ComponentModel;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.AuditTrail {
    public interface IContextAuditTrailRule : IAuditTrailRule, IContextLogicRule {
        [Category("AuditTrail")]
        [DataSourceProperty("AuditTrailMembersContexts")]
        string AuditTrailMembersContext { get; set; }
    }
}