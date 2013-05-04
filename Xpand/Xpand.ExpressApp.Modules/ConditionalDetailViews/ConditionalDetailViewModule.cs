using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using Xpand.ExpressApp.ConditionalDetailViews.Logic;
using Xpand.ExpressApp.ConditionalDetailViews.Model;
using Xpand.ExpressApp.ConditionalDetailViews.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.ConditionalDetailViews {
    [ToolboxBitmap(typeof(ConditionalDetailViewModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ConditionalDetailViewModule : LogicModuleBase<IConditionalDetailViewRule, ConditionalDetailViewRule> {
        public ConditionalDetailViewModule() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelApplication, IModelApplicationConditionalDetailView>();
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ConditionalDetailViewDefaultGroupContextNodeUpdater());
            updaters.Add(new ConditionalDetailViewRulesNodeUpdater());
            updaters.Add(new ConditionalDetailViewDefaultContextNodeUpdater());
        }
        #endregion

        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationConditionalDetailView)applicationModel).ConditionalDetailView;
        }
    }

}

