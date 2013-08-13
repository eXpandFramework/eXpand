using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public interface IObjectViewRule : IConditionalLogicRule {
        [DataSourceProperty("ObjectViews")]
        [Category("Data")]
        [Required]
        IModelObjectView ObjectView { get; set; }
    }
}
