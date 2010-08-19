using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ArtifactState.Logic {
    public abstract class ArtifactStateRule:ConditionalLogicRule,IArtifactStateRule {
        protected ArtifactStateRule(IArtifactStateRule artifactStateRule)
            : base(artifactStateRule)
        {
            Module=artifactStateRule.Module;
        }

        public string Module { get; set; }
    }
}