using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
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

        protected override IEnumerable<AdditionalViewControlsRule> CollectRulesFromModelCore(
            AdditionalViewControlsRulesNodeWrapper wrapper, ITypeInfo typeInfo) {
            IEnumerable<AdditionalViewControlsRule> collectRulesFromModelCore = base.CollectRulesFromModelCore(wrapper,
                                                                                                               typeInfo);
            EnsureControlTypeIsNotNull(collectRulesFromModelCore);
            EnsureDecoratorTypeIsNotNull(collectRulesFromModelCore);
            return collectRulesFromModelCore;
        }

        void EnsureDecoratorTypeIsNotNull(IEnumerable<AdditionalViewControlsRule> additionalViewControlsRules) {
            IEnumerable<AdditionalViewControlsRule> additionalViewControlsRulesWithNoControlType =
                additionalViewControlsRules.Where(rule => rule.DecoratorType == null);
            foreach (
                AdditionalViewControlsRule additionalViewControlsRule in additionalViewControlsRulesWithNoControlType) {
                additionalViewControlsRule.DecoratorType = GetDecoratorType();
            }
        }

        protected abstract Type GetDecoratorType();

        void EnsureControlTypeIsNotNull(IEnumerable<AdditionalViewControlsRule> collectRulesFromModelCore) {
            IEnumerable<AdditionalViewControlsRule> additionalViewControlsRulesWithNoControlType =
                collectRulesFromModelCore.Where(rule => rule.ControlType == null);
            foreach (
                AdditionalViewControlsRule additionalViewControlsRule in additionalViewControlsRulesWithNoControlType) {
                additionalViewControlsRule.ControlType = GetControlType();
            }
        }

        protected abstract Type GetControlType();

        protected override string GetMoreSchema() {
            return new SchemaHelper().Serialize<IAdditionalViewControlsRule>(false);
        }

        protected override string GetElementNodeName() {
            return AdditionalViewControlsRuleNodeWrapper.NodeNameAttribute;
        }
            }
}