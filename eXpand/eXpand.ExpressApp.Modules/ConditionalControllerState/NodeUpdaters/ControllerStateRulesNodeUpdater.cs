using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.ConditionalControllerState.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalControllerState.NodeUpdaters {
    public class ControllerStateRulesNodeUpdater :
        LogicRulesNodeUpdater<IControllerStateRule, IModelControllerStateRule, IModelArtifactState>
    {
        protected override void SetAttribute(IModelControllerStateRule rule,
                                             IControllerStateRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelArtifactState, object>> ExecuteExpression() {
            return state => state.ConditionalControllerState;
        }
    }
}