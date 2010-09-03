using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ConditionalDetailViews.Logic;
using eXpand.ExpressApp.ConditionalDetailViews.Model;
using eXpand.ExpressApp.ConditionalDetailViews.NodeUpdaters;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ConditionalDetailViews
{
    public sealed class ConditionalDetailViewModule : LogicModuleBase<IConditionalDetailViewRule, ConditionalDetailViewRule>
    {
        public ConditionalDetailViewModule()
        {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelApplication, IModelApplicationConditionalDetailView>();
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ConditionalDetailViewDefaultGroupContextNodeUpdater());
            updaters.Add(new ConditionalDetailViewRulesNodeUpdater());
            updaters.Add(new ConditionalDetailViewDefaultContextNodeUpdater());
        }
        #endregion

        protected override IModelLogic GetModelLogic(IModelApplication applicationModel)
        {
            return ((IModelApplicationConditionalDetailView)applicationModel).ConditionalDetailView;
        }
    }

}

