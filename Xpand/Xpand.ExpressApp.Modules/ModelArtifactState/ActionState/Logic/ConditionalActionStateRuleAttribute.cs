using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ConditionalActionState.Logic {
    public class ConditionalActionStateRuleAttribute : ConditionalLogicRuleAttribute, IConditionalActionStateRule {
        public ConditionalActionStateRuleAttribute(string id, string actionId, string normalCriteria, string emptyCriteria, ActionState actionState) : base(id, normalCriteria, emptyCriteria) {
            ActionState = actionState;
            ActionId = actionId;
        }

        public string ActionId { get; set; }
        public ActionState ActionState { get; set; }
        public string Module { get; set; }
    }
}