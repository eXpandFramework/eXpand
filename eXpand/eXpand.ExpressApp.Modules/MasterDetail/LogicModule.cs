using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.MasterDetail.Logic;
using eXpand.ExpressApp.MasterDetail.Model;
using eXpand.ExpressApp.MasterDetail.NodeUpdaters;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.MasterDetail {
    public class MasterDetailModule : LogicModuleBase<IMasterDetailRule, MasterDetailRule>
    {
        public MasterDetailModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelApplication, IModelApplicationMasterDetail>();
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new MasterDetailDefaultGroupContextNodeUpdater());
            updaters.Add(new MasterDetailRulesNodeUpdater());
            updaters.Add(new MasterDetailDefaultContextNodeUpdater());
        }
        #endregion
        protected override IModelLogic GetModelLogic(IModelApplication applicationModel)
        {
            return ((IModelApplicationMasterDetail)applicationModel).MasterDetail;
        }
    }
}