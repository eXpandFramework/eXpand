using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using RuleDefaultMessageTemplates = Xpand.Persistent.Base.Validation.RuleDefaultMessageTemplates;

namespace Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired
{
    [GenerateMessageTemplatesModel("RuleRequiredForAtLeast1Property")]
    [DomainComponent]
    public interface IRuleRequiredForAtLeast1PropertyProperties:IRuleMultiPropertiesValues
    {
        [DefaultValue(RuleDefaultMessageTemplates.TargetProertiesMustNotBeEmpty)]
        [Category("Format")][Localizable(true)]
        string MessageTemplateMustNotBeEmpty { get; set; }
    }
}