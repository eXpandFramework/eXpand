using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    public class AdditionalViewControlsRulesNodeUpdater :LogicRulesNodeUpdater<IAdditionalViewControlsRule, IModelAdditionalViewControlsRule> {
        protected override void SetAttribute(IModelAdditionalViewControlsRule rule,IAdditionalViewControlsRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplication, object>> ExecuteExpression() {
            return application => ((IModelApplicationAdditionalViewControls) application).AdditionalViewControls;
        }
    }
}