using System.Collections.Generic;
using System.ComponentModel;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.ExpressApp.ConditionalActionState.Model;
using Xpand.ExpressApp.ConditionalActionState.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalActionState {

    [ToolboxItem(false)]
    public sealed class ConditionalActionStateModule : LogicModuleBase<IActionStateRule, ActionStateRule,IModelActionStateRule,IModelArtifactState,IModelLogicConditionalActionState>{

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ExecutionContext.ViewChanging}; }
        }

        public override LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule, IModelArtifactState> LogicRulesNodeUpdater {
            get { return new ActionStateRulesNodeUpdater(); }
        }

        public override IModelLogicConditionalActionState GetModelLogic(IModelArtifactState modelArtifactState) {
            return modelArtifactState.ConditionalActionState;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.FindTypeInfo(typeof(IModelActionStateRule)).AddAttribute(new ModelEditorLogicRuleAttribute(typeof(IModelLogicConditionalActionState)));
        }

        #region IModelExtender Members


        #endregion
    }
}