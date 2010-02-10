using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers {
    public class ActionStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper, IActionStateRule {
        public const string ActionIdAttribute = "ActionId";
        public const string NodeNameAttribute = "ActionStateRule";

        public ActionStateRuleNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) {
        }

        public ActionStateRuleNodeWrapper(DictionaryNode ruleNode)
            : base(ruleNode) {
        }

        #region IActionStateRule Members
        public string ActionId {
            get { return Node.GetAttributeValue(ActionIdAttribute, null); }
            set { Node.SetAttribute(ActionIdAttribute, value); }
        }
        #endregion
        public override State State
        {
            get { return (State) GetEnumValue(StateAttribute, ActionState.Default); }
            set { Node.SetAttribute(StateAttribute, value.ToString()); }
        }

        public override string NodeName {
            get { return NodeNameAttribute; }
        }
    }
}