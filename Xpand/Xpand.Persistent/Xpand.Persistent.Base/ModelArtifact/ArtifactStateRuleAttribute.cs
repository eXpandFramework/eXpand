using Xpand.Persistent.Base.Logic;

namespace Xpand.Persistent.Base.ModelArtifact {
    public abstract class ArtifactStateRuleAttribute : LogicRuleAttribute, IArtifactStateRule {
        protected ArtifactStateRuleAttribute(string id, string normalCriteria, string emptyCriteria)
            : base(id, normalCriteria, emptyCriteria) {
        }
        #region IArtifactStateRule Members
        public string Module { get; set; }

        #endregion
    }
}