using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces
{
    public interface IArtifactStateRule:IStateRule
    {
        [DisplayName("Module (regex)")]
        string Module { get; set; }

        
    }
}