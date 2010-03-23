using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    public class ControllerStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper, IControllerStateRule {
        public const string StateAttribute = "State";
        public const string ControllerTypeAttribute = "ControllerType";
        public const string NodeNameAttribute = "ControllerStateRule";

        public ControllerStateRuleNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) {
        }

        public ControllerStateRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode) {
        }

        public ControllerStateRulesNodeWrapper ActionStateRulesNodeWrapper {
            get {
                if (Node.Parent != null) {
                    return new ControllerStateRulesNodeWrapper(Node.Parent);
                }
                return null;
            }
        }

        public override string NodeName {
            get { return NodeNameAttribute; }
        }
        #region IControllerStateRule Members
        public string ControllerType {
            get { return Node.GetAttributeValue(ControllerTypeAttribute, null); }
            set { Node.SetAttribute(ControllerTypeAttribute, value); }
        }

        public State State {
            get { return GetEnumValue(StateAttribute,State.Disabled); }
            set { Node.SetAttribute(StateAttribute,value.ToString()); }
        }
        #endregion
    }
}