using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.MasterDetail {
    public class MasterDetailLogicInstaller : LogicInstaller<IMasterDetailRule, IModelMasterDetailRule> {
        public MasterDetailLogicInstaller(XpandModuleBase xpandModuleBase) : base(xpandModuleBase) {
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext> { ExecutionContext.ObjectSpaceObjectChanged, ExecutionContext.CurrentObjectChanged, ExecutionContext.ControllerActivated }; }
        }

        public override LogicRulesNodeUpdater<IMasterDetailRule, IModelMasterDetailRule> LogicRulesNodeUpdater {
            get { return new MasterDetailRulesNodeUpdater(); }
        }

        public override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationMasterDetail) applicationModel).MasterDetail;
        }
    }
}