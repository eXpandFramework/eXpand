using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects
{
    [VisibleInReports(false)]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, "User Model")]
    public class XpoUserModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStore
    {
        private bool nonPersistent;
        public const string BasicUsersAssociation = "XpoDictionaryDifferenceStoreBasicUsers";

        public XpoUserModelDictionaryDifferenceStore(Session session) : base(session)
        {
        }
        public bool NonPersistent
        {
            get { return nonPersistent; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref nonPersistent, value); }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            if (Session.IsNewObject(this))
                DifferenceType=DifferenceType.User;
        }
        
    }

    public class XpoUserModelDictionaryDifferenceStoreBuilder : XpoModelDictionaryDifferenceStoreBuilder
    {
        public static IQueryable<XpoModelDictionaryDifferenceStore> GetActiveStores(Session session, IOrderedQueryable<XpoModelDictionaryDifferenceStore> stores, string clientType)
        {
            
            var containsOperator = new ContainsOperator("Users",new BinaryOperator("Oid",((XPBaseObject)SecuritySystem.CurrentUser)
                                                                         .ClassInfo.KeyProperty.GetValue(SecuritySystem.CurrentUser)));
            var stores1 = new XPCollection<XpoUserModelDictionaryDifferenceStore>(session,
                                                                                  containsOperator);
            IEnumerable<Guid> list = new List<Guid> {Guid.Empty};

            foreach (XpoUserModelDictionaryDifferenceStore store in stores1)
                ((List<Guid>) list).Add(store.Oid);
            IQueryable<XpoModelDictionaryDifferenceStore> queryable = from store in stores
                                   where
                                       store is
                                       XpoUserModelDictionaryDifferenceStore
                                   select store;
            
            return queryable.Where(store => list.Contains(store.Oid));
//            return
//                (IOrderedQueryable<XpoModelDictionaryDifferenceStore>)
//                (list.Count() > 0 ? from store in stores where list.Contains(store.Oid) select store : stores);
        }


        public new static XpoUserModelDictionaryDifferenceStore GetActiveStore(Session session, DifferenceType differenceType, string applicationTypeName)
        {
            var activeStores = GetActiveStores(true,session, differenceType, applicationTypeName);
            return
                (XpoUserModelDictionaryDifferenceStore) GetActiveStores(session, activeStores, applicationTypeName).Take(1).FirstOrDefault();
        }
    }
}