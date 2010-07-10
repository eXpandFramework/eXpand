using System;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRuleAttribute:ConditionalLogicRuleAttribute,IAdditionalViewControlsRule {
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
    }
}