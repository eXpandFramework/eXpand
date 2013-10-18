using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    [ModelAbstractClass]
    public interface IArtifactStateRule:ILogicRule {
        [Category("Data")]
        [DataSourceProperty("Modules")]
        string Module { get; set; }
    }
}