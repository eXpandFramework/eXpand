using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public abstract class AdditionalViewControlsProviderModuleBase :
        ModelRuleProviderModuleBase<AdditionalViewControlsAttribute, AdditionalViewControlsRulesNodeWrapper,
            AdditionalViewControlsRuleNodeWrapper, AdditionalViewControlsRuleInfo, AdditionalViewControlsRule> {
        protected override string ModelRulesNodeAttributeName {
            get { return AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute; }
        }

        protected override string GetMoreSchema() {
            return new SchemaHelper().Serialize<IAdditionalViewControlsRule>(false);
        }

        protected override string GetElementNodeName() {
            return AdditionalViewControlsRuleNodeWrapper.NodeNameAttribute;
        }
    }
}