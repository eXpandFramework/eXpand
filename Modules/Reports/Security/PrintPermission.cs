using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;

namespace eXpand.ExpressApp.Reports.Security
{
    [NonPersistent]
    public class PrintPermission:ControllersStatePermission
    {
        public override IPermission Copy()
        {
            return new PrintPermission();
        }

        
    }
}