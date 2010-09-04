using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ArtifactState.Logic;

namespace Xpand.ExpressApp.ConditionalActionState.Logic {
    public interface IActionStateRule : IArtifactStateRule {
        [Required]
        [DataSourceProperty("Actions")]
        [Category("Data")]
        string ActionId { get; set; }

        [Category("Behavior")]
        ActionState ActionState { get; set; }
    }
}