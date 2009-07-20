using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Permissions
{
    public abstract class ModelArtifactStatePermission:StatePermission,IModuleRule
    {
        [DisplayName("Module (regex)")]
        public string Module { get; set; }

    }
}