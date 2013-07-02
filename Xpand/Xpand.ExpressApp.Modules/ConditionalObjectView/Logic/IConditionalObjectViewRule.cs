using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ConditionalObjectView.Logic {
    public interface IConditionalObjectViewRule : IConditionalLogicRule {
        [DataSourceProperty("ObjectViews")]
        [Category("Data")]
        [Required]
        IModelObjectView ObjectView { get; set; }
    }
}
