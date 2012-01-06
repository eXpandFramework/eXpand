using System;
using System.Drawing;
using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic.Conditional.Security;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security {
    [NonPersistent]
    public class AdditionalViewControlsPermission : ConditionalLogicRulePermission, IAdditionalViewControlsRule {
        [RuleRequiredField]
        public Type ControlType { get; set; }
        [RuleRequiredField]
        public Type DecoratorType { get; set; }

        #region IAdditionalViewControlsRule Members

        public string Message { get; set; }
        public string MessageProperty { get; set; }
        public Position Position { get; set; }
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public FontStyle? FontStyle { get; set; }
        public int? Height { get; set; }
        public int? FontSize { get; set; }
        #endregion
        public override IPermission Copy() {
            return new AdditionalViewControlsPermission();
        }
    }
}