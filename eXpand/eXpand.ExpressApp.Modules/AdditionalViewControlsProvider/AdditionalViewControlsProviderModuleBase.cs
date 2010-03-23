using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public abstract class AdditionalViewControlsProviderModuleBase : ConditionalLogicRuleProviderModuleBase<IAdditionalViewControlsRule>
    {
        public override string LogicRulesNodeAttributeName {
            get { return AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute; }
        }

        public override string GetElementNodeName() {
            return AdditionalViewControlsRuleNodeWrapper.NodeNameAttribute;
        }
        void EnsureDecoratorTypeIsNotNull(IEnumerable<IAdditionalViewControlsRule> additionalViewControlsRules) {
            IEnumerable<IAdditionalViewControlsRule> additionalViewControlsRulesWithNoControlType =
                additionalViewControlsRules.Where(rule => rule.DecoratorType == null);
            foreach (IAdditionalViewControlsRule additionalViewControlsRule in additionalViewControlsRulesWithNoControlType) {
                additionalViewControlsRule.DecoratorType = GetDecoratorType();
            }
        }

        protected abstract Type GetDecoratorType();

        void EnsureControlTypeIsNotNull(IEnumerable<IAdditionalViewControlsRule> collectRulesFromModelCore) {
            IEnumerable<IAdditionalViewControlsRule> additionalViewControlsRulesWithNoControlType =
                collectRulesFromModelCore.Where(rule => rule.ControlType == null);
            foreach (
                IAdditionalViewControlsRule additionalViewControlsRule in additionalViewControlsRulesWithNoControlType) {
                additionalViewControlsRule.ControlType = GetControlType();
            }
        }
        protected override void OnCollectedRulesFromModel(CollectedRuleFromModelEventArgs<IAdditionalViewControlsRule> e)
        {
            base.OnCollectedRulesFromModel(e);
            EnsureControlTypeIsNotNull(e.LogicRules);
            EnsureDecoratorTypeIsNotNull(e.LogicRules);
        }

        protected abstract Type GetControlType();

        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var dictionaryNode = model.RootNode.FindChildElementByPath(AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute+"/Rules");
            var additionalViewControlsRulesNodeWrapper = new AdditionalViewControlsRulesNodeWrapper((DictionaryNode) dictionaryNode);
            EnsureControlTypeIsNotNull(additionalViewControlsRulesNodeWrapper.Rules);
            EnsureDecoratorTypeIsNotNull(additionalViewControlsRulesNodeWrapper.Rules);
        }
    }


}