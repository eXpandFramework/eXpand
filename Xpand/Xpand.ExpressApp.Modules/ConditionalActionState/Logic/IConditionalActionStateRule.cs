using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ConditionalActionState.Logic {
    public interface IConditionalActionStateRule : IConditionalLogicRule {
        [Required]
        [DataSourceProperty("Actions")]
        [Category("Data")]
        string ActionId { get; set; }

        [Category("Behavior")]
        ActionState ActionState { get; set; }
        [Category("Data")]
        [DataSourceProperty("Modules")]
        string Module { get; set; }
    }
}