using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class AdditionalViewControlsAttribute : ConditionalLogicRuleAttribute,IAdditionalViewControlsRule
    {
        public AdditionalViewControlsAttribute(string id,string message, 
                                               AdditionalViewControlsProviderPosition
                                                   additionalViewControlsProviderPosition, Type controlType,
                                               Type decoratorType,  string messagePropertyName, string normalCriteria, string emptyCriteria) : base(id,Nesting.Any,normalCriteria,emptyCriteria,ViewType.Any,
                                                                                                                                                              null) {
            Message = message;
            AdditionalViewControlsProviderPosition = additionalViewControlsProviderPosition;
            ControlType = controlType;
            DecoratorType = decoratorType;
            MessagePropertyName = messagePropertyName;
        }
        public AdditionalViewControlsAttribute(string id,string message, 
                                               string messagePropertyName, string normalCriteria, string emptyCriteria) : this(id, message, AdditionalViewControlsProviderPosition.Top,
                                                                                                                               null,
                                                                                                                               null,
                                                                                                                               messagePropertyName, normalCriteria, emptyCriteria)
        {
            
        }

        public string Message { get; set; }
        public string MessagePropertyName { get; set; }
        public Type DecoratorType { get; set; }
        public Type ControlType { get; set; }
        public AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition { get; set; }
        public bool UseSameIfFound { get; set; }
    }
}