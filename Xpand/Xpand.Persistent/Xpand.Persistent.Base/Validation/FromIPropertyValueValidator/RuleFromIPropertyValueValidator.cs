﻿using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    public class RuleFromIPropertyValueValidator : RulePropertyValue {
        public const string PropertiesMessageTemplateInvalidPropertyValue = "MessageTemplateInvalidPropertyValue";

        static IValueManager<string> _defaultMessageTemplateInvalidPropertyValue;


        public RuleFromIPropertyValueValidator() {
        }

        public RuleFromIPropertyValueValidator(IRulePropertyValueProperties properties) : base(properties) {
        }

        public static string DefaultMessageTemplateInvalidPropertyValue {
            get {
                if (_defaultMessageTemplateInvalidPropertyValue == null)

                    _defaultMessageTemplateInvalidPropertyValue = new SimpleValueManager<string>();

                return _defaultMessageTemplateInvalidPropertyValue.Value ??
                       (_defaultMessageTemplateInvalidPropertyValue.Value = "Invalid {TargetPropertyName}");
            }

            set {
                if (_defaultMessageTemplateInvalidPropertyValue == null)

                    _defaultMessageTemplateInvalidPropertyValue = new SimpleValueManager<string>();

                _defaultMessageTemplateInvalidPropertyValue.Value = value;
            }
        }

        public new IRuleFromIPropertyValueValidatorProperties Properties {
            get { return (IRuleFromIPropertyValueValidatorProperties) base.Properties; }
        }

        public override Type PropertiesType {
            get { return typeof (RuleFromIPropertyValueValidatorProperties); }
        }

        protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
            errorMessageTemplate = null;
            bool result = ((IPropertyValueValidator) target).IsPropertyValueValid(Properties.TargetPropertyName,ref errorMessageTemplate,Properties.TargetContextIDs, Id);
            if (errorMessageTemplate == null)
                errorMessageTemplate = Properties.MessageTemplateInvalidPropertyValue;

            return result;
        }
    }
}