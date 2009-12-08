using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.ModelArtifactState.StateRules;
using eXpand.ExpressApp.Security.NodeWrappers;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public abstract class ArtifactStateRuleNodeWrapper : DictionaryStateNodeWrapperBase,IArtifactStateRule
    {
        
        public const string ModuleAttribute = "Module";
        



        protected ArtifactStateRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode)
        {
        }
        public override string ToString()
        {
            if (ModelArtifactStateNodeWrapper != null)
            {
                return string.Format("{0}({1},{2},{3},{4},{5})[{6}]", NodeName, State, NormalCriteria, EmptyCriteria, ViewType, Module, ModelArtifactStateNodeWrapper);
            }
            return base.ToString();
        }

        public abstract string NodeName { get; }

        public string Module
        {
            get { return Node.GetAttributeValue(ModuleAttribute); }
            set { Node.SetAttribute(ModuleAttribute, value); }
        }

        public ModelArtifactStateNodeWrapper ModelArtifactStateNodeWrapper
        {
            get
            {
                if (Node.Parent != null){
                    return new ModelArtifactStateNodeWrapper(Node.Parent);
                }
                return null;
            }
        }

        public static explicit operator ArtifactStateRule(ArtifactStateRuleNodeWrapper artifactStateRuleNodeWrapper)
        {
            return artifactStateRuleNodeWrapper is ControllerStateRuleNodeWrapper
                       ? (ArtifactStateRule) new ControllerStateRule(artifactStateRuleNodeWrapper)
                       : new ActionStateRule(artifactStateRuleNodeWrapper);
        }

    }
}