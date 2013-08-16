using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public class ObjectViewRulesNodeUpdater :LogicRulesNodeUpdater<IObjectViewRule, IModelObjectViewRule> {
        protected override void SetAttribute(IModelObjectViewRule rule, IObjectViewRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplication, IModelLogic>> ExecuteExpression() {
            return application => ((IModelApplicationConditionalObjectView)application).ConditionalObjectView;
        }
    }
}