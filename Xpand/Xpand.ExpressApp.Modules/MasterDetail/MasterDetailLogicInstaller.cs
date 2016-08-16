using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.MasterDetail;

namespace Xpand.ExpressApp.MasterDetail {
    public class MasterDetailLogicInstaller : LogicInstaller<IMasterDetailRule, IModelMasterDetailRule> {
        public MasterDetailLogicInstaller(XpandModuleBase xpandModuleBase) : base(xpandModuleBase) {
        }

        public override List<ExecutionContext> ExecutionContexts => new List<ExecutionContext> { ExecutionContext.ObjectSpaceObjectChanged, ExecutionContext.CurrentObjectChanged, ExecutionContext.ControllerActivated };

        public override LogicRulesNodeUpdater<IMasterDetailRule, IModelMasterDetailRule> LogicRulesNodeUpdater => new MasterDetailRulesNodeUpdater();

        protected override IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel) {
            var modelLogicMasterDetail = ((IModelApplicationMasterDetail) applicationModel).MasterDetail;
            return new ModelLogicWrapper(modelLogicMasterDetail.Rules, modelLogicMasterDetail.ExecutionContextsGroup,
                                         modelLogicMasterDetail.ViewContextsGroup,
                                         modelLogicMasterDetail.FrameTemplateContextsGroup);
        }
    }
}