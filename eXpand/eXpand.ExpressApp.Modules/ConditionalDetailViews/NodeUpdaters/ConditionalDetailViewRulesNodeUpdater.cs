using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.ConditionalDetailViews.Logic;
using eXpand.ExpressApp.ConditionalDetailViews.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalDetailViews.NodeUpdaters {
    public class ConditionalDetailViewRulesNodeUpdater :
        LogicRulesNodeUpdater<IConditionalDetailViewRule, IModelConditionalDetailViewRule, IModelApplicationConditionalDetailView>
    {
        


        protected override void SetAttribute(IModelConditionalDetailViewRule rule, IConditionalDetailViewRule attribute) {
            rule.Attribute=attribute;
        }

        protected override Expression<Func<IModelApplicationConditionalDetailView, object>> ExecuteExpression()
        {
            return controls => controls.ConditionalDetailView;
        }
    }
}