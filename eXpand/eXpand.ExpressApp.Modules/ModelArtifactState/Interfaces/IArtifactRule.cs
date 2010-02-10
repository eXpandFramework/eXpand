using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Attributes;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces
{
    public interface IArtifactRule:IStateRule
    {
        [DisplayName("Module (regex)")]
        string Module { get; set; }

        
    }
}