using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl.Validation.RuleMultiPropertiesValues;

namespace eXpand.Persistent.BaseImpl.Validation.CombinationOfPropertiesIsUnique
{
    [NonPersistent]
    public class RuleCombinationOfPropertiesIsUniqueProperties : RuleSearchObjectProperties, IRuleMultiPropertiesValues
    {
        public string TargetProperties { get; set; }

        public string Delimiters { get; set; }
    }
}