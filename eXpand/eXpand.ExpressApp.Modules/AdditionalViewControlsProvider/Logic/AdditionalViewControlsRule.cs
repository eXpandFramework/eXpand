using System;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRule : ConditionalLogicRule, IAdditionalViewControlsRule {
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

        public Type ControlType {
            get { return _additionalViewControlsRule.ControlType; }
            set { _additionalViewControlsRule.ControlType = value; }
        }


        public Type DecoratorType {
            get { return _additionalViewControlsRule.DecoratorType; }
            set { _additionalViewControlsRule.DecoratorType = value; }
        }


        public string MessageProperty {
            get { return _additionalViewControlsRule.MessageProperty; }
            set { _additionalViewControlsRule.MessageProperty = value; }
        }


        public Position Position {
            get { return _additionalViewControlsRule.Position; }
            set { _additionalViewControlsRule.Position = value; }
        }

        public bool NotUseSameType {
            get { return _additionalViewControlsRule.NotUseSameType; }
            set { _additionalViewControlsRule.NotUseSameType=value; }
        }
        #endregion
    }
}