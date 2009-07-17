using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property
{
    [NonPersistent]
    public class RuleRequiredForAtLeast1PropertyProperties : RuleMultiPropertiesValues.RuleMultiPropertiesValues, IRuleRequiredForAtLeast1PropertyProperties
    {
        public string MessageTemplateMustNotBeEmpty { get; set; }
        
    }

}