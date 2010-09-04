using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation {
    public interface IRuleMultiPropertiesValues : IRuleBaseProperties {
        [Required]
        [Category("Data")]
        string TargetProperties { get; set; }

        [DefaultValue(";,.:")]
        [Category("Data")]
        [Required]
        string Delimiters { get; set; }
    }
}