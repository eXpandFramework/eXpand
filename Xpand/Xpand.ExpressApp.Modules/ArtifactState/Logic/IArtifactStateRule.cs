using System.ComponentModel;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ArtifactState.Logic {
    public interface IArtifactStateRule:IConditionalLogicRule {
        [Category("Data")]
        [DataSourceProperty("Modules")]
        string Module { get; set; }
    }
}