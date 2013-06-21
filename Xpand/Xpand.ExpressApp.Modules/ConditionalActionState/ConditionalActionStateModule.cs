using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.ExpressApp.ConditionalActionState.Model;
using Xpand.ExpressApp.ConditionalActionState.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalActionState {

    [ToolboxItem(false)]
    public sealed class ConditionalActionStateModule : LogicModuleBase<IActionStateRule, ActionStateRule> {

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ActionStateDefaultGroupContextNodeUpdater());
            updaters.Add(new ActionStateDefaultContextNodeUpdater());
            updaters.Add(new ActionStateRulesNodeUpdater());
        }

        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationModelArtifactState)applicationModel).ModelArtifactState.ConditionalActionState;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.FindTypeInfo(typeof(IModelLogicConditionalActionState)).AddAttribute(new ModelLogicRuleAttribute(typeof(IModelActionStateRule)));
        }

        #region IModelExtender Members


        #endregion
    }
}