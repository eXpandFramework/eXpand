using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.ExpressApp.ConditionalActionState.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalActionState.NodeUpdaters {
    public class ActionStateRulesNodeUpdater :
        LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule, IModelApplicationModelArtifactState> {
        protected override void SetAttribute(IModelActionStateRule rule,
                                             IActionStateRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplicationModelArtifactState, object>> ExecuteExpression() {
            return state => state.ModelArtifactState.ConditionalActionState;
        }
    }
}