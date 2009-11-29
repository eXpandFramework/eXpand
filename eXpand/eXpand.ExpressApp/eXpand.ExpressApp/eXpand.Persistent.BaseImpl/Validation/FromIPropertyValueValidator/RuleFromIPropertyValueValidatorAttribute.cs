using System;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.BaseImpl.Validation.FromIPropertyValueValidator
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RuleFromIPropertyValueValidatorAttribute : RuleBaseAttribute,
                                                            IRuleFromIPropertyValueValidatorProperties
    {
        public RuleFromIPropertyValueValidatorAttribute(string id, string targetContextIDs)
            : base(id, targetContextIDs)
        {
        }

        public RuleFromIPropertyValueValidatorAttribute(string id, DefaultContexts targetContexts)
            : base(id, targetContexts)
        {
        }

        public RuleFromIPropertyValueValidatorAttribute(string id, string targetContextIDs, string messageTemplate)
            : base(id, targetContextIDs, messageTemplate)
        {
        }

        public RuleFromIPropertyValueValidatorAttribute(string id, DefaultContexts targetContexts,
                                                        string messageTemplate)
            : base(id, targetContexts, messageTemplate)
        {
        }

        protected new RuleFromIPropertyValueValidatorProperties Properties
        {
            get { return (RuleFromIPropertyValueValidatorProperties) base.Properties; }
        }

        protected override Type RuleType
        {
            get { return typeof (RuleFromIPropertyValueValidator); }
        }

        protected override Type PropertiesType
        {
            get { return typeof (RuleFromIPropertyValueValidatorProperties); }
        }
        #region IRuleFromIPropertyValueValidatorProperties Members
        string IRulePropertyValueProperties.TargetPropertyName
        {
            get { return Properties.TargetPropertyName; }

            set { Properties.TargetPropertyName = value; }
        }

        string IRuleFromIPropertyValueValidatorProperties.MessageTemplateInvalidPropertyValue
        {
            get { return Properties.MessageTemplateInvalidPropertyValue; }

            set { Properties.MessageTemplateInvalidPropertyValue = value; }
        }
        #endregion

    }
}