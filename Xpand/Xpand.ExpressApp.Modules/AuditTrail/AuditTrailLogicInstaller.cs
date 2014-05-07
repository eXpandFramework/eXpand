using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail {
    public class AuditTrailLogicInstaller : LogicInstaller<IAuditTrailRule, IModelAuditTrailRule> {
        public AuditTrailLogicInstaller(IXpandModuleBase xpandModuleBase)
            : base(xpandModuleBase) {
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>(); }
        }

        public override LogicRulesNodeUpdater<IAuditTrailRule, IModelAuditTrailRule> LogicRulesNodeUpdater {
            get { return new AuditTrailRulesNodeUpdater(); }
        }

        protected override IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel) {
            var auditTrail = ((IModelApplicationAudiTrail)applicationModel).AudiTrail;
            return new ModelLogicWrapper(auditTrail.Rules, null,auditTrail.ViewContextsGroup, auditTrail.FrameTemplateContextsGroup);
        }
    }
}
