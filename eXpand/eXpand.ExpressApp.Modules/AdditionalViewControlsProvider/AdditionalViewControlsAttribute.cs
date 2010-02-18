using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class AdditionalViewControlsAttribute : ModelRuleAttribute
    {
        private readonly string message;
        private readonly string messagePropertyName;
        private readonly AdditionalViewControlsProviderPosition additionalViewControlsProviderPosition;
        private readonly Type controlType;
        private readonly Type decoratorType;

        public AdditionalViewControlsAttribute(string message, 
                                               AdditionalViewControlsProviderPosition
                                                   additionalViewControlsProviderPosition, Type controlType,
                                               Type decoratorType, string id, Nesting nesting, string normalCriteria, string emptyCriteria, ViewType viewType, string viewId, string messagePropertyName) : base(id,nesting,normalCriteria,emptyCriteria,viewType,viewId) {
            this.message = message;
            this.messagePropertyName = messagePropertyName;
            this.additionalViewControlsProviderPosition = additionalViewControlsProviderPosition;
            this.controlType = controlType;
            this.decoratorType = decoratorType;
//            if (!typeof (AdditionalViewControlsProviderDecorator).IsAssignableFrom(decoratorType))
//                throw new ArgumentException("decoratorType");
        }

        public Type DecoratorType
        {
            get { return decoratorType; }
        }

        public Type ControlType
        {
            get { return controlType; }
        }

        public AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition
        {
            get { return additionalViewControlsProviderPosition; }
        }

        public string Message
        {
            get { return message; }
        }


        public string MessagePropertyName
        {
            get { return messagePropertyName; }
        }

    }
}