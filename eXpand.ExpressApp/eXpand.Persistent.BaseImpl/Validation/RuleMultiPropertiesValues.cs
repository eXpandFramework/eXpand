using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.Validation.RuleMultiPropertiesValues
{
    [NonPersistent]
    public abstract class RuleMultiPropertiesValues : RuleBaseProperties, IRuleMultiPropertiesValues
    {
        [RulePropertiesRequired, RulePropertiesLocalized]
        public string TargetProperties { get; set; }
        public string Delimiters { get; set; }
        
    }
}