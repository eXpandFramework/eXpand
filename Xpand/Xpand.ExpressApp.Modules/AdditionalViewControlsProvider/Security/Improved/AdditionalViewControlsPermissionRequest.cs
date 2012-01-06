using System;
using System.Drawing;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic.Conditional.Security.Improved;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security.Improved {
    public class AdditionalViewControlsPermissionRequest : ConditionalLogicRulePermissionRequest, IAdditionalViewControlsRule {
        public AdditionalViewControlsPermissionRequest(string operation, IAdditionalViewControlsRule logicRule)
            : base(operation, logicRule) {
            ControlType = logicRule.ControlType;
            DecoratorType = logicRule.DecoratorType;
            Message = logicRule.Message;
            Position = logicRule.Position;
            BackColor = logicRule.BackColor;
            ForeColor = logicRule.ForeColor;
            FontStyle = logicRule.FontStyle;
            Height = logicRule.Height;
            FontSize = logicRule.FontSize;
        }
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
    }
}