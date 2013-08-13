using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Model {
    public class ControllerStateRulesNodeUpdater :
        LogicRulesNodeUpdater<IControllerStateRule, IModelControllerStateRule> {
        protected override void SetAttribute(IModelControllerStateRule rule,
                                             IControllerStateRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplication, object>> ExecuteExpression() {
            return state => ((IModelApplicationModelArtifactState) state).ModelArtifactState.ConditionalControllerState;
        }
    }
}