using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public abstract class AdditionalViewControlsProviderModuleBase :ModuleBase    {
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
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            var additionalViewControlsProviderModule =
                (AdditionalViewControlsProviderModule)
                moduleManager.Modules.FindModule(typeof (AdditionalViewControlsProviderModule));
            additionalViewControlsProviderModule.CollectedRulesFromModel+=AdditionalViewControlsProviderModuleOnCollectedRulesFromModel;            
        }

        void AdditionalViewControlsProviderModuleOnCollectedRulesFromModel(object sender, CollectedRuleFromModelEventArgs<IAdditionalViewControlsRule> collectedRuleFromModelEventArgs) {
            EnsureControlTypeIsNotNull(collectedRuleFromModelEventArgs.LogicRules);
            EnsureDecoratorTypeIsNotNull(collectedRuleFromModelEventArgs.LogicRules);
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