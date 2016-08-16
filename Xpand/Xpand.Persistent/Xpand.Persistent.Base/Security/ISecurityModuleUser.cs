using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.Security {
    public interface ISecurityModuleUser {
        
    }

    public static class SecurityModuleUserLogic {
        public static void AddSecurityObjectsToAdditionalExportedTypes(this ISecurityModuleUser securityModuleUser,string nameSpace){
            var xpandModuleBase = ((XpandModuleBase) securityModuleUser);
            if (!xpandModuleBase.RuntimeMode)
                xpandModuleBase.AddToAdditionalExportedTypes(nameSpace);
            else if (typeof(IPermissionPolicyUser).IsAssignableFrom(xpandModuleBase.Application.Security.UserType)) {
                xpandModuleBase.AddToAdditionalExportedTypes(nameSpace);
            }
        }

    }
}
