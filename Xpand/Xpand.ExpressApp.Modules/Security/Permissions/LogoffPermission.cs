using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security.Permissions {
    public class AnonymousLoginPermission : ModifierPermission {
        public const string OperationName = "AnonymousLogin";
        public AnonymousLoginPermission(Modifier modifier)
            : base(OperationName) {
            Modifier = modifier;
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
    [System.ComponentModel.DisplayName(AnonymousLoginPermission.OperationName)]
    public class AnonymousLoginOperationPermissionData : ModifierPermissionData {
        public AnonymousLoginOperationPermissionData(Session session)
            : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new AnonymousLoginPermission(Modifier) };
        }
    }
    [Serializable]
    public class AnonymousLoginOperationRequest : ModifierOperationRequest {
        public AnonymousLoginOperationRequest(AnonymousLoginPermission permission)
            : base(permission, AnonymousLoginPermission.OperationName) {
        }
    }

    public class AnonymousLoginRequestProcessor : ModifierRequestProcessor<AnonymousLoginPermission> {
        public AnonymousLoginRequestProcessor(IPermissionDictionary permissions)
            : base(permissions) {
        }
    }

}
