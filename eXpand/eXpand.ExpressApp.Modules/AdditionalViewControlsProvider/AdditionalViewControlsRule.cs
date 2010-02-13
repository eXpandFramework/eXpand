using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public class AdditionalViewControlsRule : ModelRule, IAdditionalViewControlsRule {
        readonly IAdditionalViewControlsRule _additionalViewControlsRule;

        public AdditionalViewControlsRule(IAdditionalViewControlsRule additionalViewControlsRule)
            : base(additionalViewControlsRule) {
            _additionalViewControlsRule = additionalViewControlsRule;
        }
        #region IAdditionalViewControlsRule Members
        public string Message {
            get { return _additionalViewControlsRule.Message; }
            set { _additionalViewControlsRule.Message = value; }
        }

        public string MessagePropertyName {
            get { return _additionalViewControlsRule.MessagePropertyName; }
            set { _additionalViewControlsRule.MessagePropertyName = value; }
        }

        public Type DecoratorType {
            get { return _additionalViewControlsRule.DecoratorType; }
            set { _additionalViewControlsRule.DecoratorType = value; }
        }

        public Type ControlType {
            get { return _additionalViewControlsRule.ControlType; }
            set { _additionalViewControlsRule.ControlType = value; }
        }

        public AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition {
            get { return _additionalViewControlsRule.AdditionalViewControlsProviderPosition; }
            set { _additionalViewControlsRule.AdditionalViewControlsProviderPosition = value; }
        }

        public bool UseSameIfFound {
            get { return _additionalViewControlsRule.UseSameIfFound; }
            set { _additionalViewControlsRule.UseSameIfFound=value; }
        }
        #endregion
    }
}