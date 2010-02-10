using DevExpress.ExpressApp;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers {
    public class AdditionalViewControlsRulesNodeWrapper : ModelRulesNodeWrapper<AdditionalViewControlsRuleNodeWrapper, AdditionalViewControlsAttribute>
    {
        public const string NodeNameAttribute = "AdditionalViewControls";

        public AdditionalViewControlsRulesNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }

        protected override string ChildNodeName {
            get { return AdditionalViewControlsRuleNodeWrapper.NodeNameAttribute; }
        }
        public override AdditionalViewControlsRuleNodeWrapper AddRule(AdditionalViewControlsAttribute additionalViewControlsAttribute, DevExpress.ExpressApp.DC.ITypeInfo typeInfo) {
            AdditionalViewControlsRuleNodeWrapper additionalViewControlsRuleNodeWrapper = base.AddRule(additionalViewControlsAttribute, typeInfo);
            additionalViewControlsRuleNodeWrapper.AdditionalViewControlsProviderPosition =additionalViewControlsAttribute.AdditionalViewControlsProviderPosition;
            additionalViewControlsRuleNodeWrapper.Message=additionalViewControlsAttribute.Message;
            additionalViewControlsRuleNodeWrapper.MessagePropertyName =additionalViewControlsRuleNodeWrapper.MessagePropertyName;            
            return additionalViewControlsRuleNodeWrapper;
        }
    }
}