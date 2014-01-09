using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    [NonPersistent]
    public class RuleFromIPropertyValueValidatorProperties : RulePropertyValueProperties,
                                                             IRuleFromIPropertyValueValidatorProperties {
        #region IRuleFromIPropertyValueValidatorProperties Members
        [Category("Format")]
        [Localizable(true)]
        public string MessageTemplateInvalidPropertyValue { get; set; }
        #endregion

    }
}