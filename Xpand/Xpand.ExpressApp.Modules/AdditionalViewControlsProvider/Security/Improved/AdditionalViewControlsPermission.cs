using System;
using System.Collections.Generic;
using System.Drawing;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security.Improved {
    public class AdditionalViewControlsPermission : LogicRulePermission, IContextAdditionalViewControlsRule {
        public const string OperationName = "AdditionalViewControls";
        public AdditionalViewControlsPermission(IContextAdditionalViewControlsRule logicRule)
            : base(OperationName, logicRule) {
            ControlType=logicRule.ControlType;
            DecoratorType = logicRule.DecoratorType;
            Message = logicRule.Message;
            Position=logicRule.Position;
            BackColor=logicRule.BackColor;
            ForeColor=logicRule.ForeColor;
            FontStyle=logicRule.FontStyle;
            Height=logicRule.Height;
            FontSize=logicRule.FontSize;
        }
        public Type ControlType { get; set; }
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
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
}
