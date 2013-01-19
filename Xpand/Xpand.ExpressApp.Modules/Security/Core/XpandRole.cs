using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Security.Permissions;
using System.Linq;

namespace Xpand.ExpressApp.Security.Core {
    public interface IObjectReadOperationPermission : IObjectOperationPermission {
    }
    public interface IObjectNavigateOperationPermission : IObjectOperationPermission {
    }

    public interface IObjectOperationPermission {
    }
    [ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class XpandRole : SecuritySystemRole {
        public XpandRole(Session session)
            : base(session) {
        }

        protected override IEnumerable<IOperationPermission> GetPermissionsCore() {
            var operationPermissions = base.GetPermissionsCore().Union(Permissions.SelectMany(data => data.GetPermissions()));
            var permissions = operationPermissions.Union(PermissionProviderStorage.Instance.SelectMany(info => info.GetPermissions(this)));
            var xpMemberInfos = ClassInfo.OwnMembers.Where(info => info.IsAssociationList);
            return xpMemberInfos.Aggregate(permissions, OperationPermissions);
        }

        IEnumerable<IOperationPermission> OperationPermissions(IEnumerable<IOperationPermission> permissions, XPMemberInfo member) {
            var collection = (XPBaseCollection)member.GetValue(this);
            var operationPermissions = new List<IOperationPermission>();
            foreach (IObjectOperationPermission operationPermission in collection) {
                if (typeof(IObjectReadOperationPermission).IsAssignableFrom(member.CollectionElementType.ClassType))
                    AddOperationPermission(member, operationPermissions, operationPermission, "Read");
                if (typeof(IObjectNavigateOperationPermission).IsAssignableFrom(member.CollectionElementType.ClassType))
                    AddOperationPermission(member, operationPermissions, operationPermission, "Navigate");
            }
            return permissions.Union(operationPermissions);
        }

        void AddOperationPermission(XPMemberInfo member, List<IOperationPermission> operationPermissions, IObjectOperationPermission operationPermission, string operation) {
            operationPermissions.Add(new ObjectOperationPermission(member.CollectionElementType.ClassType, Criteria(operationPermission), operation));
        }

        string Criteria(IObjectOperationPermission operationPermission) {
            var keyProperty = Session.GetClassInfo(operationPermission).KeyProperty;
            var keyValue = keyProperty.GetValue(operationPermission);
            return CriteriaOperator.Parse(keyProperty.Name + "=?", keyValue).ToString();
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
