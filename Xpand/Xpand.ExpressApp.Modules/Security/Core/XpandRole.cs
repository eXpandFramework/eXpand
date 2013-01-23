using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Security.Permissions;
using System.Linq;

namespace Xpand.ExpressApp.Security.Core {

    [ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class XpandRole : SecuritySystemRole {
        public XpandRole(Session session)
            : base(session) {
        }

        protected override IEnumerable<IOperationPermission> GetPermissionsCore() {
            var operationPermissions = base.GetPermissionsCore().Union(Permissions.SelectMany(data => data.GetPermissions()));
            var permissions = operationPermissions.Union(PermissionProviderStorage.Instance.SelectMany(info => info.GetPermissions(this)));
            var collectionMembers = OperationPermissionCollectionMembers();
            return collectionMembers.Aggregate(permissions, (current, xpMemberInfo) => current.Union(xpMemberInfo.ObjectOperationPermissions(this).Cast<IOperationPermission>()));
        }

        IEnumerable<XPMemberInfo> OperationPermissionCollectionMembers() {
            return ClassInfo.OwnMembers.Where(info => info.IsAssociationList && typeof(IObjectOperationPermission).IsAssignableFrom(info.CollectionElementType.ClassType));
        }

        public override string ToString() {
            return Name;
        }

        [Association("XpandRole-XpandPermissionDatas")]
        public XPCollection<XpandPermissionData> Permissions {
            get {
                return GetCollection<XpandPermissionData>("Permissions");
            }
        }
    }
}
