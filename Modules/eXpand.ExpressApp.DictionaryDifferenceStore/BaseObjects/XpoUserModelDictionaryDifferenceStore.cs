﻿using System;
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
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, "Μοντέλο χρηστών")]
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
        
//        [Association(BasicUsersAssociation)]
//        public XPCollection<BasicUser> BasicUsers
//        {
//            get { return GetCollection<BasicUser>(MethodBase.GetCurrentMethod().Name.Replace("get_", "")); }
//        }
    }

    public class XpoUserModelDictionaryDifferenceStoreBuilder : XpoModelDictionaryDifferenceStoreBuilder
    {
        public static IOrderedQueryable<XpoModelDictionaryDifferenceStore> GetActiveStores(Session session, IOrderedQueryable<XpoModelDictionaryDifferenceStore> stores, string clientType)
        {
//            IQueryable<XpoUserModelDictionaryDifferenceStore> queryable =
//                from store1 in new XPQuery<XpoUserModelDictionaryDifferenceStore>(session)
////                let value = (IList) store1.GetMemberValue(typeof (BasicUser).Name + "s")
////                where store1.BasicUsers.Contains((BasicUser) SecuritySystem.CurrentUser)
//                where ttt(store1).Contains(SecuritySystem.CurrentUser)
//                select store1;
            var stores1 = new XPCollection<XpoUserModelDictionaryDifferenceStore>(session,
                                                                                  new ContainsOperator("Users",
                                                                                                       new BinaryOperator("Oid",
                                                                                                                          ((XPBaseObject)
                                                                                                                           SecuritySystem
                                                                                                                               .
                                                                                                                               CurrentUser)
                                                                                                                              .ClassInfo.KeyProperty.GetValue(SecuritySystem
                                                                                                                                                                  .
                                                                                                                                                                  CurrentUser))));
            IEnumerable<Guid> list = new List<Guid>();

            foreach (XpoUserModelDictionaryDifferenceStore store in stores1)
                ((List<Guid>) list).Add(store.Oid);
            stores = (IOrderedQueryable<XpoModelDictionaryDifferenceStore>) (from store in stores
                                                                             where
                                                                                 store is
                                                                                 XpoUserModelDictionaryDifferenceStore
                                                                             select store);
            return
                (IOrderedQueryable<XpoModelDictionaryDifferenceStore>)
                (list.Count() > 0 ? from store in stores where list.Contains(store.Oid) select store : stores);
        }


        public new static XpoUserModelDictionaryDifferenceStore GetActiveStore(Session session, DifferenceType differenceType, string clientType)
        {
            var activeStores = GetActiveStores(true,session, differenceType, clientType);
            return
                (XpoUserModelDictionaryDifferenceStore) GetActiveStores(session, activeStores, clientType).Take(1).FirstOrDefault();
        }
    }
}