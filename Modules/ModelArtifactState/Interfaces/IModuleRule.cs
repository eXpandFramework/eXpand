using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces
{
    public interface IModuleRule:IStateRule
    {
        [DisplayName("Module (regex)")]
        string Module { get; set; }
    }
}