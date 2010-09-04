using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalControllerState.NodeUpdaters {
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