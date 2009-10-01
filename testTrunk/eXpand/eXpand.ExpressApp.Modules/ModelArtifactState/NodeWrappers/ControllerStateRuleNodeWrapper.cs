using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public class ControllerStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper,IControllerStateRule
    {
        public const string ControllerTypeAttribute = "ControllerType";
        public const string NodeNameAttribute = "ControllerStateRule";

        public ControllerStateRuleNodeWrapper() : this(new DictionaryNode( NodeNameAttribute))
        {
        }

        public ControllerStateRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode)
        {
        }
        public string ControllerType
        {
            get { return Node.GetAttributeValue(ControllerTypeAttribute, null); }
            set { Node.SetAttribute(ControllerTypeAttribute, value); }
        }


        public override string NodeName{
            get { return NodeNameAttribute; }
        }
    }
}