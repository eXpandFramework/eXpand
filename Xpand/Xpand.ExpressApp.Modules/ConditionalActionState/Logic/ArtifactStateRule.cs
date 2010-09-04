using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ArtifactState.Logic {
    public abstract class ArtifactStateRule:ConditionalLogicRule,IArtifactStateRule {
        readonly IArtifactStateRule _artifactStateRule;

        protected ArtifactStateRule(IArtifactStateRule artifactStateRule)
            : base(artifactStateRule)
        {
            _artifactStateRule = artifactStateRule;
        }

        public string Module {
            get { return _artifactStateRule.Module; }
            set { _artifactStateRule.Module=value; }
        }
    }
}