using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.Persistent.BaseImpl.Security.PermissionPolicyData{
    [System.ComponentModel.DisplayName(AnonymousLoginPermission.OperationName)]
    public class AnonymousLoginOperationPermissionPolicyData : ModifierPermissionPolicyData {
        public AnonymousLoginOperationPermissionPolicyData(Session session)
            : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new AnonymousLoginPermission(Modifier) };
        }
    }
}