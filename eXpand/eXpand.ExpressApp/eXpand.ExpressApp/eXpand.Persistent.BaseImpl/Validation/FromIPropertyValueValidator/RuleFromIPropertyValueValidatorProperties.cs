using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.Validation.FromIPropertyValueValidator
{
    [NonPersistent]
    public class RuleFromIPropertyValueValidatorProperties : RulePropertyValueProperties,
                                                             IRuleFromIPropertyValueValidatorProperties
    {
        #region IRuleFromIPropertyValueValidatorProperties Members
        public string MessageTemplateInvalidPropertyValue { get; set; }
        #endregion
        
    }
}