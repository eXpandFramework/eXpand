using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public class ObjectViewRulesNodeUpdater :
        LogicRulesNodeUpdater<IObjectViewRule, IModelObjectViewRule> {
        protected override void SetAttribute(IModelObjectViewRule rule, IObjectViewRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplication, object>> ExecuteExpression() {
            return application => ((IModelApplicationConditionalObjectView)application).ConditionalObjectView;
        }
    }
}