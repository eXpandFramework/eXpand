using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.ExpressApp.DictionaryDifferenceStore.Controllers;
using eXpand.ExpressApp.DictionaryDifferenceStore.Security;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores
{
    public class XpoUserModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStoreBase, IApplicationModelDiffStore
    {
        private readonly Dictionary applicationModel;

        public XpoUserModelDictionaryDifferenceStore(Session session, XafApplication application)
            : base(session, application)
        {
            applicationModel = application.Model.Clone();
        }

        public Dictionary ApplicationModel
        {
            get { return applicationModel; }
        }

        public override DifferenceType DifferenceType
        {
            get { return DifferenceType.User; }
        }

        protected override bool AllowCustomPersistence
        {
            get { return SecuritySystem.CurrentUser!= null; }
        }

        protected override BaseObjects.XpoModelDictionaryDifferenceStore GetActiveStore()
        {
            return XpoUserModelDictionaryDifferenceStoreBuilder.GetActiveStore(Session, DifferenceType, application.GetType().FullName);
        }

        protected override IOrderedQueryable<BaseObjects.XpoModelDictionaryDifferenceStore> GetActiveStores(
            bool queryAspect)
        {
            if (SecuritySystem.CurrentUser!= null)
            {
                IOrderedQueryable<BaseObjects.XpoModelDictionaryDifferenceStore> stores = base.GetActiveStores(queryAspect);
                return XpoUserModelDictionaryDifferenceStoreBuilder.GetActiveStores(Session, stores, application.GetType().FullName);
            }
            return base.GetActiveStores(queryAspect);
        }

        protected override BaseObjects.XpoModelDictionaryDifferenceStore getNewStore(Session session)
        {
            return new BaseObjects.XpoUserModelDictionaryDifferenceStore(session);
        }

        protected override void OnXpoUserModelDictionaryDifferenceStoreSaving(
            BaseObjects.XpoModelDictionaryDifferenceStore xpoUserModelDictionaryDifferenceStore)
        {
            var xpoUserModelDictionaryDifferenceStore1 =
                ((BaseObjects.XpoUserModelDictionaryDifferenceStore) xpoUserModelDictionaryDifferenceStore);
            if (!xpoUserModelDictionaryDifferenceStore1.NonPersistent)
            {
                associateCurrentUserWithModel(xpoUserModelDictionaryDifferenceStore, xpoUserModelDictionaryDifferenceStore1);
                base.OnXpoUserModelDictionaryDifferenceStoreSaving(xpoUserModelDictionaryDifferenceStore);
            }
            if (SecuritySystem.IsGranted(new CombineModelChangesWithApplicationModelPermission(CombineModelChangesWithApplicationModelModifier.Allow)))
            {
                BaseObjects.XpoModelDictionaryDifferenceStore modelDictionaryDifferenceStore =
                    XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(Session, DifferenceType.Model,
                                                                            application.GetType().FullName);
                if (modelDictionaryDifferenceStore != null)
                {
                    CombineXpoDictionaryDifferenceStoreViewController.Combine(
                        new List<BaseObjects.XpoModelDictionaryDifferenceStore> {modelDictionaryDifferenceStore},
                        xpoUserModelDictionaryDifferenceStore1, this);
                    modelDictionaryDifferenceStore.Save();
                }
            }
        }

        private void associateCurrentUserWithModel(BaseObjects.XpoModelDictionaryDifferenceStore xpoUserModelDictionaryDifferenceStore, BaseObjects.XpoUserModelDictionaryDifferenceStore xpoUserModelDictionaryDifferenceStore1)
        {
            ((IList)xpoUserModelDictionaryDifferenceStore1.GetMemberValue("Users")).Add(
                xpoUserModelDictionaryDifferenceStore.Session.GetObjectByKey(
                    SecuritySystem.UserType,
                    ((XPBaseObject) SecuritySystem.CurrentUser).ClassInfo.KeyProperty.GetValue(
                        SecuritySystem.CurrentUser)));
        }

        public Dictionary GetActiveApplicationModel()
        {
            return new Dictionary(XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(Session, DifferenceType.Model, application.GetType().FullName).Model);
        }
    }
}