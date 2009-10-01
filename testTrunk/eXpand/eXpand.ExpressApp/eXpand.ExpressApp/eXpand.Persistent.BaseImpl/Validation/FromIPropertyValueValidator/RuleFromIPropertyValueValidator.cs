using System;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.BaseImpl.Validation.FromIPropertyValueValidator
{
    public class RuleFromIPropertyValueValidator : RulePropertyValue
    {
        public const string PropertiesMessageTemplateInvalidPropertyValue = "MessageTemplateInvalidPropertyValue";

        private static IValueManager<string> defaultMessageTemplateInvalidPropertyValue;

        public RuleFromIPropertyValueValidator(string id, PropertyInfo property, ContextIdentifiers targetContextIDs)
            : base(id, property, targetContextIDs, property.DeclaringType)
        {
        }

        public RuleFromIPropertyValueValidator()
        {
        }

        public RuleFromIPropertyValueValidator(RulePropertyValueProperties properties) : base(properties)
        {
        }

        public static string DefaultMessageTemplateInvalidPropertyValue
        {
            get
            {
                if (defaultMessageTemplateInvalidPropertyValue == null)

                    defaultMessageTemplateInvalidPropertyValue = new SimpleValueManager<string>();

                if (defaultMessageTemplateInvalidPropertyValue.Value == null)

                    defaultMessageTemplateInvalidPropertyValue.Value = "Invalid {TargetPropertyName}";

                return defaultMessageTemplateInvalidPropertyValue.Value;
            }

            set
            {
                if (defaultMessageTemplateInvalidPropertyValue == null)

                    defaultMessageTemplateInvalidPropertyValue = new SimpleValueManager<string>();

                defaultMessageTemplateInvalidPropertyValue.Value = value;
            }
        }

        public new RuleFromIPropertyValueValidatorProperties Properties
        {
            get { return (RuleFromIPropertyValueValidatorProperties) base.Properties; }
        }

        public override Type PropertiesType
        {
            get { return typeof (RuleFromIPropertyValueValidatorProperties); }
        }

        protected override bool IsValidInternal(object target, out string errorMessageTemplate)
        {
            errorMessageTemplate = null;

            
            
            bool result = ((IPropertyValueValidator) target).IsPropertyValueValid(Properties.TargetPropertyName,
                                                                                  ref errorMessageTemplate, Properties.TargetContextIDs,Id);

            if (errorMessageTemplate == null)

                errorMessageTemplate = Properties.MessageTemplateInvalidPropertyValue;

            return result;
        }
    }
}