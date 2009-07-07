using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.BaseImpl.Validation.RuleMultiPropertiesValues
{
    public abstract class RuleMultiPropertiesValues : RuleBaseProperties, IRuleMultiPropertiesValues
    {
        [RulePropertiesRequired, RulePropertiesLocalized]
        public string TargetProperties { get; set; }
        public string Delimiters { get; set; }
        
    }
}