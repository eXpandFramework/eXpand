using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries
{
    public class QueryRoleModelDifferenceObject:QueryDifferenceObject<RoleModelDifferenceObject>
    {
        public QueryRoleModelDifferenceObject(Session session) : base(session){
        }


        public override IQueryable<RoleModelDifferenceObject> GetActiveModelDifferences(string applicationName){
            var userWithRoles = SecuritySystem.CurrentUser as IUserWithRoles;
            if (userWithRoles != null)
            {
                IEnumerable<object> collection =
                    userWithRoles.Roles.Cast<XPBaseObject>().Select(role => role.ClassInfo.KeyProperty.GetValue(role));
                Type roleType = ((ISecurityComplex)SecuritySystem.Instance).RoleType;
                ITypeInfo roleTypeInfo = XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type == roleType).Single();
                var criteria = new ContainsOperator("Roles", new InOperator(roleTypeInfo.KeyMember.Name, collection.ToList()));

                
                var roleAspectObjects = base.GetActiveModelDifferences(applicationName).ToList();
                return roleAspectObjects.OfType<RoleModelDifferenceObject>().Where(aspectObject => aspectObject.Fit(criteria.ToString())).AsQueryable();
            }
            return new List<RoleModelDifferenceObject>().AsQueryable();

        }
    }
}
