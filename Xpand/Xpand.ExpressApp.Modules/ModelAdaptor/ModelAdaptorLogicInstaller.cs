using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor {
    public class ModelAdaptorLogicInstaller:LogicInstaller<IModelAdaptorRule,IModelModelAdaptorRule>,IModelAdaptorLogicIntaller {
        public ModelAdaptorLogicInstaller(XpandModuleBase xpandModuleBase) : base(xpandModuleBase) {
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext> { ExecutionContext.ControllerActivated }; }
        }

        public override LogicRulesNodeUpdater<IModelAdaptorRule, IModelModelAdaptorRule> LogicRulesNodeUpdater {
            get { return new ModelAdaptorRulesNodeUpdater(); }
        }

        protected override IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel) {
            var modelModelAdaptorLogic = ((IModelApplicationModelAdaptor) applicationModel).ModelAdaptor;
            return new ModelLogicWrapper(modelModelAdaptorLogic.Rules, modelModelAdaptorLogic);
        }

    }
}