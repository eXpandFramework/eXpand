using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRulesNodeWrapper : ConditionalLogicRulesNodeWrapper<IAdditionalViewControlsRule>
    {
        public const string NodeNameAttribute = "AdditionalViewControls";

        public AdditionalViewControlsRulesNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }

        protected override string ChildNodeName {
            get { return AdditionalViewControlsRuleNodeWrapper.NodeNameAttribute; }
        }
        public override IAdditionalViewControlsRule AddRule(IAdditionalViewControlsRule logicRuleAttribute, ITypeInfo typeInfo, Type logicRuleNodeWrapper) {
            IAdditionalViewControlsRule additionalViewControlsRuleNodeWrapper = base.AddRule(logicRuleAttribute, typeInfo, logicRuleNodeWrapper);
            additionalViewControlsRuleNodeWrapper.AdditionalViewControlsProviderPosition = logicRuleAttribute.AdditionalViewControlsProviderPosition;
            additionalViewControlsRuleNodeWrapper.Message = logicRuleAttribute.Message;
            additionalViewControlsRuleNodeWrapper.MessagePropertyName = logicRuleAttribute.MessagePropertyName;
            additionalViewControlsRuleNodeWrapper.ControlType = logicRuleAttribute.ControlType;
            additionalViewControlsRuleNodeWrapper.DecoratorType = logicRuleAttribute.DecoratorType;            
            additionalViewControlsRuleNodeWrapper.UseSameIfFound = logicRuleAttribute.UseSameIfFound;            
            return additionalViewControlsRuleNodeWrapper;
        }
    }
}