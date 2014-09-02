using System.ComponentModel;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired {
    [GenerateMessageTemplatesModel("RuleRequiredForAtLeast1Property")]
    public interface IRuleRequiredForAtLeast1PropertyProperties : IRuleMultiPropertiesValues {
        [DefaultValue(RuleDefaultMessageTemplates.TargetProertiesMustNotBeEmpty)]
        [Category("Format")]
        [Localizable(true)]
        string MessageTemplateMustNotBeEmpty { get; set; }
    }
}