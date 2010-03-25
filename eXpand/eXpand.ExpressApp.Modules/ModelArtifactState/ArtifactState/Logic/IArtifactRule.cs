using DevExpress.Xpo;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public interface IArtifactRule : IConditionalLogicRule {
        [DisplayName("Module (regex)")]
        string Module { get; set; }
    }
}