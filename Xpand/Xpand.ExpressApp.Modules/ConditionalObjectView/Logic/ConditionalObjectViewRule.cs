using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ConditionalObjectView.Logic {
    public class ConditionalObjectViewRule : ConditionalLogicRule, IConditionalObjectViewRule {
        public ConditionalObjectViewRule(IConditionalObjectViewRule conditionalObjectViewRule)
            : base(conditionalObjectViewRule) {
            ObjectView = conditionalObjectViewRule.ObjectView;
        }

        public IModelObjectView ObjectView { get; set; }
    }
}
