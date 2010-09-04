using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base.Validation {
    [NonPersistent]
    public abstract class RuleMultiPropertiesValues : RuleBaseProperties, IRuleMultiPropertiesValues {
        #region IRuleMultiPropertiesValues Members
        public string TargetProperties { get; set; }
        public string Delimiters { get; set; }
        #endregion
    }
}