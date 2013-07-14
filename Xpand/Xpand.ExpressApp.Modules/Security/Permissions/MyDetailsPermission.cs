using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.Permissions {
    public class MyDetailsPermission : ModifierPermission {
        public const string OperationName = "MyDetails";
        public MyDetailsPermission(Modifier modifier)
            : base(OperationName) {
            Modifier = modifier;
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
    [System.ComponentModel.DisplayName(MyDetailsPermission.OperationName)]
    public class MyDetailsOperationPermissionData : ModifierPermissionData {
        public MyDetailsOperationPermissionData(Session session)
            : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new MyDetailsPermission(Modifier) };
        }
    }
    [Serializable]
    public class MyDetailsOperationRequest : ModifierOperationRequest {
        public MyDetailsOperationRequest(MyDetailsPermission permission)
            : base(permission, MyDetailsPermission.OperationName) {
        }
    }

    public class MyDetailsRequestProcessor : ModifierRequestProcessor<MyDetailsPermission> {
        public MyDetailsRequestProcessor(IPermissionDictionary permissions)
            : base(permissions) {
        }
    }

}
