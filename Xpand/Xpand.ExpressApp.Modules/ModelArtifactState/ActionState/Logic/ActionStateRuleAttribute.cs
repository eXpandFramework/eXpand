using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public sealed class ActionStateRuleAttribute : ArtifactStateRuleAttribute, IContextActionStateRule {
        public ActionStateRuleAttribute(string id, string actionId, string normalCriteria, string emptyCriteria, ActionState actionState) : base(id, normalCriteria, emptyCriteria) {
            ActionState = actionState;
            ActionId = actionId;
        }

        public string ActionId { get; set; }
        public ActionState ActionState { get; set; }
    }
}