using System;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRuleAttribute:ConditionalLogicRuleAttribute,IAdditionalViewControlsRule {
        public AdditionalViewControlsRuleAttribute(string id, string normalCriteria, string emptyCriteria, Type controlType, Type decoratorType, string message, Position position, bool useSameIfFound) : base(id, normalCriteria, emptyCriteria) {
            UseSameIfFound = true;
            ControlType = controlType;
            DecoratorType = decoratorType;
            Message = message;
            Position = position;
            UseSameIfFound = useSameIfFound;
        }
        public AdditionalViewControlsRuleAttribute(string id, string normalCriteria, string emptyCriteria,  string message, Position position, bool useSameIfFound) : base(id, normalCriteria, emptyCriteria) {
            UseSameIfFound = true;
            Message = message;
            Position = position;
            UseSameIfFound = useSameIfFound;
        }


        public Type ControlType { get; set; }
        public Type DecoratorType { get; set; }
        public string MessageProperty { get; set; }
        public string Message { get; set; }
        public Position Position { get; set; }
        public bool UseSameIfFound { get; set; }
    }
}