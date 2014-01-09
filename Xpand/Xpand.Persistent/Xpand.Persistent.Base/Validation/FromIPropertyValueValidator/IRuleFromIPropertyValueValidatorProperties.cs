using System.ComponentModel;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    [GenerateMessageTemplatesModel("RuleFromIPropertyValueValidatorProperties")]
    public interface IRuleFromIPropertyValueValidatorProperties : IRulePropertyValueProperties {
        [Category("Format")]
        [Localizable(true)]
        [DefaultValue(RuleDefaultMessageTemplates.InvalidTargetPropertyName)]
        string MessageTemplateInvalidPropertyValue { get; set; }
    }
}