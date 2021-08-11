using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries {
    public class QueryRoleModelDifferenceObject : QueryDifferenceObject<RoleModelDifferenceObject> {
        public QueryRoleModelDifferenceObject(Session session)
            : base(session) {
        }
        public override IQueryable<RoleModelDifferenceObject> GetActiveModelDifferences(string applicationName, string name, DeviceCategory deviceCategory=DeviceCategory.All) {
            var userWithRoles = SecuritySystem.CurrentUser as IPermissionPolicyUser;
            var collection = GetRoles(userWithRoles);
            if (collection != null) {
                var roleType = ((IRoleTypeProvider)SecuritySystem.Instance).RoleType;
                var roleTypeInfo = XafTypesInfo.Instance.PersistentTypes.First(info => info.Type == roleType);
                var criteria = new ContainsOperator("Roles", new InOperator(roleTypeInfo.KeyMember.Name, collection.ToList()));
                var roleAspectObjects = base.GetActiveModelDifferences(applicationName, name,deviceCategory).ToArray();
                return roleAspectObjects.Where(aspectObject => aspectObject.Fit(criteria.ToString())).AsQueryable();
            }
            return base.GetActiveModelDifferences(applicationName, name,deviceCategory).OfType<RoleModelDifferenceObject>().AsQueryable();
        }

        IEnumerable<object> GetRoles(IPermissionPolicyUser userWithRoles) {
            IEnumerable<object> roles = null;
            if (userWithRoles != null) {
                roles = userWithRoles.Roles.OfType<XPBaseObject>().Select(role => role.ClassInfo.KeyProperty.GetValue(role));
            }
            if (roles == null) {
                if (SecuritySystem.CurrentUser is ISecurityUserWithRoles securityUserWithRoles) {
                    roles =securityUserWithRoles.Roles.OfType<XPBaseObject>().Select(role => role.ClassInfo.KeyProperty.GetValue(role));
                }
            }
            return roles;
        }
    }
}
