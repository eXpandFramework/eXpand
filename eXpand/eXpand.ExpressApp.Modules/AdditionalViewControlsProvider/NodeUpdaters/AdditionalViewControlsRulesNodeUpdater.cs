using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters {
    public class AdditionalViewControlsRulesNodeUpdater :
        LogicRulesNodeUpdater<IAdditionalViewControlsRule, IModelAdditionalViewControlsRule,IModelApplicationAdditionalViewControls> {
        protected override void SetAttribute(IModelAdditionalViewControlsRule rule,
                                             IAdditionalViewControlsRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplicationAdditionalViewControls, object>> ExecuteExpression() {
            return controls => controls.AdditionalViewControls;
        }
        }
}