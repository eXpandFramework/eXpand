using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
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
        [Association(Associations.XpoRoleModelDictionaryDifferenceStoreRoles)]
        public XPCollection<Role> Roles
        {
            get
            {
                return GetCollection<Role>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
    }

    public class XpoRoleModelDictionaryDifferenceStoreBuilder
    {
        public static IQueryable<XpoRoleModelDictionaryDifferenceStore> GetStores(Session session)
        {
            IEnumerable<Guid> collection = ((User)SecuritySystem.CurrentUser).Roles.Select(role => role.Oid);
            var queryable = new XPQuery<XpoRoleModelDictionaryDifferenceStore>(session).Where(store => store.Roles.Any(role =>
                                                                                                                              collection.Contains(role.Oid)));
            return queryable;
        }
    }
    internal class Associations
    {
        public const string XpoRoleModelDictionaryDifferenceStoreRoles = "XpoRoleModelDictionaryDifferenceStoreRoles";
    }
}