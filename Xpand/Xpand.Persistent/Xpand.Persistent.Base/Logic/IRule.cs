using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic {
    public interface IRule {
        [ModelPersistentName("ID")]
        [Category("Design")]
        [Description("Read-only. Required. Specifies the current rule identifier.")]
        string Id { get; set; }

        [Localizable(true)]
        [Category("Misc")]
        [Description("Localizable. Specifies the description of the current rule.")]
        string Description { get; set; }

        int? Index { get; set; }
    }
}