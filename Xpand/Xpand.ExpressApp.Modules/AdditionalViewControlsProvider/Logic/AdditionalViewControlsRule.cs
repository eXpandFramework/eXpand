using System;
using System.Drawing;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRule : ConditionalLogicRule, IAdditionalViewControlsRule {
        public AdditionalViewControlsRule(IAdditionalViewControlsRule additionalViewControlsRule)
            : base(additionalViewControlsRule) {
                Message = additionalViewControlsRule.Message;
                ControlType = additionalViewControlsRule.ControlType;
                DecoratorType = additionalViewControlsRule.DecoratorType;
                MessageProperty = additionalViewControlsRule.MessageProperty;
                Position = additionalViewControlsRule.Position;
                NotUseSameType = additionalViewControlsRule.NotUseSameType;
                BackColor=additionalViewControlsRule.BackColor;
        }
        #region IAdditionalViewControlsRule Members
        public string Message { get; set; }

        public Type ControlType { get; set; }


        public Type DecoratorType { get; set; }


        public string MessageProperty { get; set; }


        public Position Position { get; set; }

        public bool NotUseSameType { get; set; }

        public Color? BackColor { get; set; }

        public object Control { get; set; }
        #endregion
    }
}