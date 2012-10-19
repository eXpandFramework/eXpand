using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Core;

namespace Xpand.ExpressApp.Security.Permissions {
    public interface IPermissionInfo {
        IEnumerable<IOperationPermission> GetPermissions(XpandRole xpandRole);
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