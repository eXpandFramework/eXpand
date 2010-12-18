using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.ConditionalDetailViews.Logic;
using Xpand.ExpressApp.ConditionalDetailViews.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalDetailViews.NodeUpdaters {
    public class ConditionalDetailViewRulesNodeUpdater :
        LogicRulesNodeUpdater<IConditionalDetailViewRule, IModelConditionalDetailViewRule, IModelApplicationConditionalDetailView> {



        protected override void SetAttribute(IModelConditionalDetailViewRule rule, IConditionalDetailViewRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplicationConditionalDetailView, object>> ExecuteExpression() {
            return controls => controls.ConditionalDetailView;
        }
    }
}