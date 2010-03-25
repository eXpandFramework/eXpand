using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactStateRulesNodeWrapper<TArtifactStateRule> : ConditionalLogicRulesNodeWrapper<TArtifactStateRule> where TArtifactStateRule : IArtifactRule
    {
        protected ArtifactStateRulesNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }
        public override TArtifactStateRule AddRule(TArtifactStateRule logicRuleAttribute, DevExpress.ExpressApp.DC.ITypeInfo typeInfo, System.Type logicRuleNodeWrapper) {
            TArtifactStateRule artifactStateRule = base.AddRule(logicRuleAttribute, typeInfo, logicRuleNodeWrapper);
            artifactStateRule.Module = logicRuleAttribute.Module;
            return artifactStateRule;
        }
    }
}