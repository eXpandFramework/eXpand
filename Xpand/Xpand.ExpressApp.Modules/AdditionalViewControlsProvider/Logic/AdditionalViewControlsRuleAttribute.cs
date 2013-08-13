using System;
using System.Drawing;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRuleAttribute:LogicRuleAttribute,IAdditionalViewControlsRule {
        public AdditionalViewControlsRuleAttribute(string id, string normalCriteria, string emptyCriteria, Type controlType, Type decoratorType, string message, Position position) : base(id, normalCriteria, emptyCriteria) {
            ControlType = controlType;
            DecoratorType = decoratorType;
            Message = message;
            Position = position;
        }
        public AdditionalViewControlsRuleAttribute(string id, string normalCriteria, string emptyCriteria,  string message, Position position) : base(id, normalCriteria, emptyCriteria) {
            Message = message;
            Position = position;
        }


        public Type ControlType { get; set; }
        public Type DecoratorType { get; set; }
        public string MessageProperty { get; set; }
        public string Message { get; set; }
        public Position Position { get; set; }
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public FontStyle? FontStyle { get; set; }
        public int? Height { get; set; }
        public int? FontSize { get; set; }
    }
}