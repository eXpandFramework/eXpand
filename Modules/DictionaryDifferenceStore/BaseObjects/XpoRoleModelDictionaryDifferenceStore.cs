using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base;
using System.Linq;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects
{
    [VisibleInReports(false)]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, "Role Model")]
    public class XpoRoleModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStore
    {
        public XpoRoleModelDictionaryDifferenceStore(Session session) : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            if (Session.IsNewObject(this))
                DifferenceType = DifferenceType.Role;
        }
    }

    public class XpoRoleModelDictionaryDifferenceStoreBuilder
    {
        public static IQueryable<XpoRoleModelDictionaryDifferenceStore> GetStores(Session session)
        {
            var userWithRoles = SecuritySystem.CurrentUser as IUserWithRoles;
            if (userWithRoles != null)
            {
                IEnumerable<Guid> collection =
                    userWithRoles.Roles.Cast<BaseObject>().Select(role => role.Oid);
                Type roleType = ((ISecurityComplex) SecuritySystem.Instance).RoleType;
                return new XPCollection<XpoRoleModelDictionaryDifferenceStore>(session,
                                                                               new ContainsOperator(roleType.Name+"s", new InOperator("Oid", collection.ToList()))).AsQueryable();
            }
            return null;
        }
    }
    internal class Associations
    {
        public const string XpoRoleModelDictionaryDifferenceStoreRoles = "XpoRoleModelDictionaryDifferenceStoreRoles";
    }
}