using DevExpress.Persistent.Validation;
using eXpand.Persistent.BaseImpl.Validation.RuleMultiPropertiesValues;

namespace eXpand.Persistent.BaseImpl.Validation.CombinationOfPropertiesIsUnique
{
    public class RuleCombinationOfPropertiesIsUniqueProperties : RuleSearchObjectProperties, IRuleMultiPropertiesValues
    {
        public string TargetProperties { get; set; }

        public string Delimiters { get; set; }
    }
}