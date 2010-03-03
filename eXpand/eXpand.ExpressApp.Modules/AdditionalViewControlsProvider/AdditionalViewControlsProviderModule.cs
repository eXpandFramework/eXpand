using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public sealed partial class AdditionalViewControlsProviderModule : ConditionalLogicRuleProviderModuleBase<IAdditionalViewControlsRule>
    {
        public AdditionalViewControlsProviderModule() {
            InitializeComponent();
        }

        public override string LogicRulesNodeAttributeName
        {
            get { return AdditionalViewControlsRulesNodeWrapper.NodeNameAttribute; }
        }


        protected override bool IsDefaultContext(ExecutionContext context) {
            return true;
        }

        public override string GetElementNodeName()
        {   
            return AdditionalViewControlsRuleNodeWrapper.NodeNameAttribute;
        }
    }
}