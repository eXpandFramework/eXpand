using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    public class AdditionalViewControlsLogicInstaller:LogicInstaller<IAdditionalViewControlsRule,IModelAdditionalViewControlsRule> {
        public AdditionalViewControlsLogicInstaller(XpandModuleBase xpandModuleBase) : base(xpandModuleBase) {
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ExecutionContext.ViewChanged}; }
        }

        public override LogicRulesNodeUpdater<IAdditionalViewControlsRule, IModelAdditionalViewControlsRule> LogicRulesNodeUpdater {
            get { return new AdditionalViewControlsRulesNodeUpdater(); }
        }

        public override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationAdditionalViewControls) applicationModel).AdditionalViewControls;
        }
    }
}