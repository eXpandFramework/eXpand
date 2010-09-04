using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.ExpressApp.ConditionalActionState.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalActionState.NodeUpdaters {
    public class ActionStateRulesNodeUpdater :
        LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule, IModelArtifactState>
    {
        protected override void SetAttribute(IModelActionStateRule rule,
                                             IActionStateRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelArtifactState, object>> ExecuteExpression() {
            return state => state.ConditionalActionState;
        }
    }
}