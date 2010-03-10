using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.Logic.Conditional {
    public abstract class ConditionalLogicRuleNodeWrapper : LogicRuleNodeWrapper, IConditionalLogicRule {
        public const string NormalCriteriaAttribute = "NormalCriteria";
        public const string EmptyCriteriaAttribute = "EmptyCriteria";

        protected ConditionalLogicRuleNodeWrapper(DictionaryNode ruleNode)
            : base(ruleNode) {
        }
        #region IConditionalLogicRule Members
        public string NormalCriteria {
            get { return Node.GetAttributeValue(NormalCriteriaAttribute); }
            set { Node.SetAttribute(NormalCriteriaAttribute, value); }
        }

        public string EmptyCriteria {
            get { return Node.GetAttributeValue(EmptyCriteriaAttribute); }
            set { Node.SetAttribute(EmptyCriteriaAttribute, value); }
        }
        #endregion
    }
}