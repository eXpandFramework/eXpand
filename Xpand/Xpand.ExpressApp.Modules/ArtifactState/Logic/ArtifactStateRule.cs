using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ArtifactState.Logic {
    public abstract class ArtifactStateRule:ConditionalLogicRule,IArtifactStateRule {
        protected ArtifactStateRule(IArtifactStateRule artifactStateRule)
            : base(artifactStateRule)
        {
            Module=artifactStateRule.Module;
        }

        public string Module { get; set; }
    }
}