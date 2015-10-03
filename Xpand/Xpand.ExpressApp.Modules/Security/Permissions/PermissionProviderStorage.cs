using System.Collections.Generic;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security.Permissions {
    public interface IPermissionInfo {
        IEnumerable<IOperationPermission> GetPermissions(ISecurityRole securityRole);
    }


    public class PermissionProviderStorage : HashSet<IPermissionInfo> {
        static readonly PermissionProviderStorage _instance;

        static PermissionProviderStorage() {
            _instance = new PermissionProviderStorage();
        }

        public static PermissionProviderStorage Instance {
            get { return _instance; }
        }
    }
}