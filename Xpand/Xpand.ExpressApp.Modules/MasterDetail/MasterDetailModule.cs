using System.Collections.Generic;
using System.ComponentModel;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.ExpressApp.MasterDetail.NodeUpdaters;

namespace Xpand.ExpressApp.MasterDetail {

    [ToolboxItem(false)]
    public class MasterDetailModule : LogicModuleBase<IMasterDetailRule, MasterDetailRule, IModelMasterDetailRule, IModelApplicationMasterDetail, IModelLogicMasterDetail> {
        public MasterDetailModule() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ ExecutionContext.ObjectSpaceObjectChanged, ExecutionContext.CurrentObjectChanged, ExecutionContext.ControllerActivated }; }
        }

        public override LogicRulesNodeUpdater<IMasterDetailRule, IModelMasterDetailRule, IModelApplicationMasterDetail> LogicRulesNodeUpdater {
            get { return new MasterDetailRulesNodeUpdater(); }
        }
        #endregion
        public override IModelLogicMasterDetail GetModelLogic(IModelApplicationMasterDetail modelApplicationMasterDetail) {
            return modelApplicationMasterDetail.MasterDetail;
        }
    }
}