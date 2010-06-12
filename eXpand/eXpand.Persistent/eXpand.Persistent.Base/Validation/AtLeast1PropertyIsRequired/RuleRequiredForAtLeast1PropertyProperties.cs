using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired {
    [NonPersistent]
    public class RuleRequiredForAtLeast1PropertyProperties : RuleMultiPropertiesValues,
                                                             IRuleRequiredForAtLeast1PropertyProperties {
        #region IRuleRequiredForAtLeast1PropertyProperties Members
        public string MessageTemplateMustNotBeEmpty { get; set; }
        #endregion
                                                             }
}