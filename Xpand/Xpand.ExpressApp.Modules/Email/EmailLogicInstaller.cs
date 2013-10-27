using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Email.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Email {
    public class EmailLogicInstaller : LogicInstaller<IEmailRule, IModelEmailRule> {
        public EmailLogicInstaller(IXpandModuleBase xpandModuleBase) : base(xpandModuleBase) {
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ExecutionContext.ObjectSpaceCommited}; }
        }

        public override LogicRulesNodeUpdater<IEmailRule, IModelEmailRule> LogicRulesNodeUpdater {
            get { return new EmailRulesNodeUpdater(); }
        }

        protected override IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel) {
            IModelLogicEmail logicContexts = ((IModelApplicationEmail) applicationModel).Email;
            return new ModelLogicWrapper(logicContexts.Rules, logicContexts);
        }
    }
}