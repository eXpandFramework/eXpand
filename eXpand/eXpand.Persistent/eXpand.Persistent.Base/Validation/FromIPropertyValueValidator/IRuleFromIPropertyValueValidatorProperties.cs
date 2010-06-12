using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    [GenerateMessageTemplatesModel("RuleFromIPropertyValueValidatorProperties")]
    [DomainComponent]
    public interface IRuleFromIPropertyValueValidatorProperties : IRulePropertyValueProperties {
        [Category("Format")]
        [Localizable(true)]
        [DefaultValue(RuleDefaultMessageTemplates.InvalidTargetPropertyName)]
        string MessageTemplateInvalidPropertyValue { get; set; }
    }
}