using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.ExpressApp.ConditionalActionState.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalActionState.NodeUpdaters {
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