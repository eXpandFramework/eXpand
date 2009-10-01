using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AdditionalViewControlsAttribute : Attribute
    {
        private readonly string message;
        private readonly string messagePropertyName;
        private readonly ViewType targetViewType;
        private readonly AdditionalViewControlsProviderPosition additionalViewControlsProviderPosition;
        private readonly Type controlType;
        private readonly Type decoratorType;

        public AdditionalViewControlsAttribute(string message, ViewType targetViewType,
                                               AdditionalViewControlsProviderPosition
                                                   additionalViewControlsProviderPosition, Type controlType,
                                               Type decoratorType)
        {
            this.message = message;
            this.targetViewType = targetViewType;
            this.additionalViewControlsProviderPosition = additionalViewControlsProviderPosition;
            this.controlType = controlType;
            this.decoratorType = decoratorType;
            if (!typeof (AdditionalViewControlsProviderDecorator).IsAssignableFrom(decoratorType))
                throw new ArgumentException("decoratorType");
        }


        public AdditionalViewControlsAttribute(string message, ViewType targetViewType, string messagePropertyName)
        {
            this.message = message;
            this.targetViewType = targetViewType;
            this.messagePropertyName = messagePropertyName;
        }

        public AdditionalViewControlsAttribute(string hint, ViewType targetViewType)
            : this(hint, targetViewType, "")
        {
        }

        public AdditionalViewControlsAttribute(string hint)
            : this(hint, ViewType.Any)
        {
        }

        public AdditionalViewControlsAttribute(string message, ViewType targetViewType, AdditionalViewControlsProviderPosition additionalViewControlsProviderPosition)
        {
            this.message = message;
            this.targetViewType = targetViewType;
            this.additionalViewControlsProviderPosition = additionalViewControlsProviderPosition;
        }

        public AdditionalViewControlsAttribute(string message, string messagePropertyName, ViewType targetViewType, AdditionalViewControlsProviderPosition additionalViewControlsProviderPosition)
        {
            this.message = message;
            this.messagePropertyName = messagePropertyName;
            this.targetViewType = targetViewType;
            this.additionalViewControlsProviderPosition = additionalViewControlsProviderPosition;
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

        public ViewType TargetViewType
        {
            get { return targetViewType; }
        }

        public string MessagePropertyName
        {
            get { return messagePropertyName; }
        }

    }
}