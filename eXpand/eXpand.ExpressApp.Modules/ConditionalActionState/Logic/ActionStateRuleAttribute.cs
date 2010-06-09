using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalActionState.Logic {
    public class ActionStateRuleAttribute : ArtifactStateRuleAttribute, IActionStateRule {
        public ActionStateRuleAttribute(string id, string normalCriteria, string emptyCriteria, ActionState actionState, string actionId) : base(id, normalCriteria, emptyCriteria) {
            ActionState = actionState;
            ActionId = actionId;
        }

        public string ActionId { get; set; }
        public ActionState ActionState { get; set; }
    }
}