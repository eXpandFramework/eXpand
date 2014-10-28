using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    [ModelAbstractClass]
    public interface IArtifactStateRule:ILogicRule {
        [Category("Data")]
        [Description("Regex")]
        string Module { get; set; }
    }
}