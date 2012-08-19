using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.ExpressApp.MasterDetail.NodeUpdaters;

namespace Xpand.ExpressApp.MasterDetail {

    [ToolboxItem(false)]
    public class MasterDetailModule : LogicModuleBase<IMasterDetailRule, MasterDetailRule> {
        public MasterDetailModule() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelApplication, IModelApplicationMasterDetail>();
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new MasterDetailDefaultGroupContextNodeUpdater());
            updaters.Add(new MasterDetailRulesNodeUpdater());
            updaters.Add(new MasterDetailDefaultContextNodeUpdater());
        }
        #endregion
        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationMasterDetail)applicationModel).MasterDetail;
        }
    }
}