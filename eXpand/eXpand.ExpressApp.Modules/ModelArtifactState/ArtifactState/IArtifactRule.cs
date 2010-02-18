using DevExpress.Xpo;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState {
    public interface IArtifactRule : IModelRule {
        [DisplayName("Module (regex)")]
        string Module { get; set; }
    }
}