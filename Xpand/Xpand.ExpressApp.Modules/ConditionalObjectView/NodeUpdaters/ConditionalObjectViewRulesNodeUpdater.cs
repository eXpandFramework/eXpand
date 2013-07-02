using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.ConditionalObjectView.Logic;
using Xpand.ExpressApp.ConditionalObjectView.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalObjectView.NodeUpdaters {
    public class ConditionalObjectViewRulesNodeUpdater :
        LogicRulesNodeUpdater<IConditionalObjectViewRule, IModelConditionalObjectViewRule, IModelApplicationConditionalObjectView> {
        protected override void SetAttribute(IModelConditionalObjectViewRule rule, IConditionalObjectViewRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplicationConditionalObjectView, object>> ExecuteExpression() {
            return controls => controls.ConditionalObjectView;
        }
    }
}