using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalActionState.Logic {
    public interface IActionStateRule : IArtifactStateRule {
        [Required]
        [DataSourceProperty("Actions")]
        [Category("Data")]
        string ActionId { get; set; }

        [Category("Behavior")]
        ActionState ActionState { get; set; }
    }
}