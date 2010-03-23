using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper, IActionStateRule {
        public const string ActionIdAttribute = "ActionId";
        public const string ActionStateAttribute = "ActionState";
        public const string CaptionAttribute = "Caption";
        public const string NodeNameAttribute = "ActionStateRule";

        public ActionStateRuleNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) {
        }

        public ActionStateRuleNodeWrapper(DictionaryNode ruleNode)
            : base(ruleNode) {
        }

        public ActionStateRulesNodeWrapper ActionStateRulesNodeWrapper
        {
            get {
                if (Node.Parent != null) {
                    return new ActionStateRulesNodeWrapper(Node.Parent);
                }
                return null;
            }
        }

        #region IActionStateRule Members
        public string ActionId {
            get { return Node.GetAttributeValue(ActionIdAttribute, null); }
            set { Node.SetAttribute(ActionIdAttribute, value); }
        }

        public ActionState ActionState {
            get { return GetEnumValue(ActionStateAttribute,ActionState.Default); }
            set { Node.SetAttribute(ActionStateAttribute,value.ToString()); }
        }
        #endregion

        public override string NodeName {
            get { return NodeNameAttribute; }
        }
    }
}