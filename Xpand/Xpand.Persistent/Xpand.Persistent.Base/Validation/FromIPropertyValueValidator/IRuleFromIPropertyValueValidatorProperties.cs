using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using RuleDefaultMessageTemplates = Xpand.Persistent.Base.Validation.RuleDefaultMessageTemplates;

namespace Xpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    [GenerateMessageTemplatesModel("RuleFromIPropertyValueValidatorProperties")]
    [DomainComponent]
    public interface IRuleFromIPropertyValueValidatorProperties : IRulePropertyValueProperties {
        [Category("Format")]
        [Localizable(true)]
        [DefaultValue(RuleDefaultMessageTemplates.InvalidTargetPropertyName)]
        string MessageTemplateInvalidPropertyValue { get; set; }
    }
}