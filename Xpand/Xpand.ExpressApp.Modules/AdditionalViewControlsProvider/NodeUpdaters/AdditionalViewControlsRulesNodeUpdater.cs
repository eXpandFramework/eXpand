using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters {
    public class AdditionalViewControlsRulesNodeUpdater :
        LogicRulesNodeUpdater<IAdditionalViewControlsRule, IModelAdditionalViewControlsRule, IModelApplicationAdditionalViewControls> {
        protected override void SetAttribute(IModelAdditionalViewControlsRule rule,
                                             IAdditionalViewControlsRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplicationAdditionalViewControls, object>> ExecuteExpression() {
            return controls => controls.AdditionalViewControls;
        }
    }
}