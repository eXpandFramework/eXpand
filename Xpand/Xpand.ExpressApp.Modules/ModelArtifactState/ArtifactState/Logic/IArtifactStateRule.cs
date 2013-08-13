using System.ComponentModel;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public interface IArtifactStateRule:IConditionalLogicRule {
        [Category("Data")]
        [DataSourceProperty("Modules")]
        string Module { get; set; }
    }
}