using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.Persistent.Base.Logic.Model;
using System.Linq;

namespace Xpand.ExpressApp.AuditTrail.Model {
    [ModelInterfaceImplementor(typeof(IContextAuditTrailRule), "Attribute")]
    public interface IModelAuditTrailRule : IContextAuditTrailRule, IModelConditionalLogicRule<IAuditTrailRule> {
        [Browsable(false)]
        IEnumerable<string> AuditTrailMembersContexts { get; }
    }
    [DomainLogic(typeof(IModelAuditTrailRule))]
    public class ModelAuditTrailRuleDomainLogic {
        public static IEnumerable<string> Get_AuditTrailMembersContexts(IModelAuditTrailRule auditTrailRule) {
            var modelLogicAuditTrail = ((IModelLogicAuditTrail) auditTrailRule.Parent.Parent);
            var modelAuditTrailMembersContextGroup = modelLogicAuditTrail.AuditTrailMembersContextGroup;
            var membersContexts = modelAuditTrailMembersContextGroup.SelectMany(contexts => contexts).Where(context 
                => context.ModelClass.TypeInfo.IsAssignableFrom(auditTrailRule.TypeInfo));
            return membersContexts.GroupBy(context => context.Parent).Select(contexts => contexts.Key.GetValue<string>("Id"));
        }
    }

}
