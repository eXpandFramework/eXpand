using eXpand.Persistent.BaseImpl.Validation.RuleMultiPropertiesValues;

namespace eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property
{
    public interface IRuleRequiredForAtLeast1PropertyProperties:IRuleMultiPropertiesValues
    {
        string MessageTemplateMustNotBeEmpty { get; set; }
    }

}