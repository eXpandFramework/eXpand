using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactStateRuleAttribute : LogicRuleAttribute, IArtifactStateRule {
        protected ArtifactStateRuleAttribute(string id, string normalCriteria, string emptyCriteria)
            : base(id, normalCriteria, emptyCriteria) {
        }
        #region IArtifactStateRule Members
        public string Module { get; set; }

        #endregion
    }
}