using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries {
    public class QueryRoleModelDifferenceObject : QueryDifferenceObject<RoleModelDifferenceObject> {
        public QueryRoleModelDifferenceObject(Session session)
            : base(session) {
        }
        public override IQueryable<RoleModelDifferenceObject> GetActiveModelDifferences(string applicationName, string name) {
            var userWithRoles = SecuritySystem.CurrentUser as IUserWithRoles;
            var collection = Collection(userWithRoles);
            if (collection != null) {
                Type roleType = ((IRoleTypeProvider)SecuritySystem.Instance).RoleType;
                ITypeInfo roleTypeInfo = XafTypesInfo.Instance.PersistentTypes.First(info => info.Type == roleType);
                var criteria = new ContainsOperator("Roles", new InOperator(roleTypeInfo.KeyMember.Name, collection.ToList()));

                var roleAspectObjects = base.GetActiveModelDifferences(applicationName, name).ToList();
                return roleAspectObjects.Where(aspectObject => aspectObject.Fit(criteria.ToString())).AsQueryable();
            }
            return base.GetActiveModelDifferences(applicationName, name).OfType<RoleModelDifferenceObject>().AsQueryable();
        }

        static IEnumerable<object> Collection(IUserWithRoles userWithRoles) {
            IEnumerable<object> collection = null;
            if (userWithRoles != null) {
                collection = userWithRoles.Roles.OfType<XPBaseObject>().Select(role => role.ClassInfo.KeyProperty.GetValue(role));
            }
            if (collection == null) {
                var securityUserWithRoles = SecuritySystem.CurrentUser as ISecurityUserWithRoles;
                if (securityUserWithRoles != null) {
                    collection =
                        securityUserWithRoles.Roles.OfType<XPBaseObject>()
                                             .Select(role => role.ClassInfo.KeyProperty.GetValue(role));
                }
            }
            return collection;
        }
    }
}
