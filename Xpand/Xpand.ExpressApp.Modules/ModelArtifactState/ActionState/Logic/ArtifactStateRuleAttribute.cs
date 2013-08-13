using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ArtifactState.Logic {
    public abstract class ArtifactStateRuleAttribute : ConditionalLogicRuleAttribute, IArtifactStateRule {
        protected ArtifactStateRuleAttribute(string id, string normalCriteria, string emptyCriteria)
            : base(id, normalCriteria, emptyCriteria) {
        }
        #region IArtifactStateRule Members
        public string Module { get; set; }

        #endregion
    }
}