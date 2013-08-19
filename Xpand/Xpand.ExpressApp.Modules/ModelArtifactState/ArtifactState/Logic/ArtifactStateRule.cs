using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactStateRule : LogicRule, IArtifactStateRule {
        protected ArtifactStateRule(IContextArtifactStateRule artifactStateRule)
            : base(artifactStateRule) {
            Module = artifactStateRule.Module;
        }

        public string Module { get; set; }
    }
}