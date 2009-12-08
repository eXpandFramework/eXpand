using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public class ActionStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper,IActionStateRule
    {
        public const string ActionIdAttribute = "ActionId";
        public const string NodeNameAttribute = "ActionStateRule";

        public ActionStateRuleNodeWrapper() : this(new DictionaryNode( NodeNameAttribute))
        {
        }

        public ActionStateRuleNodeWrapper(DictionaryNode ruleNode)
            : base(ruleNode)
        {
        }
        public string ActionId
        {
            get { return Node.GetAttributeValue(ActionIdAttribute, null); }
            set { Node.SetAttribute(ActionIdAttribute, value); }
        }

        public override string NodeName{
            get { return NodeNameAttribute; }
        }
    }
}