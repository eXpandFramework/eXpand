using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects
{
    [DefaultClassOptions]
    [NavigationItem("Admin")]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, XpoModelDictionaryDifferenceStoreBuilder.Caption)]
    [Custom("IsClonable", "True")][VisibleInReports(false)]
    public class XpoModelDictionaryDifferenceStore : BaseObject
    {
        public const string DefaultAspect = "(Default language)";
        private string aspect;
//        private XPCollection<AuditDataItemPersistent> auditTrail;
        private DateTime dateCreated;
        private DifferenceType differenceType;
        private bool disable;
        private string name;
        private string xmlContent;

        public XpoModelDictionaryDifferenceStore(Session session) : base(session)
        {
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            Validator.RuleSet.Validate(this, ContextIdentifier.Save);
        }

        public DifferenceType DifferenceType
        {
            get { return differenceType; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref differenceType, value); }
        }

        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref name, value); }
        }
        
        private string applicationTypeName;
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string ApplicationTypeName
        {
            get
            {
                return applicationTypeName;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref applicationTypeName, value);
            }
        }
//        protected override void OnDeleted()
//        {
//            base.OnDeleted();
//            Session.CommitTransaction();
//            var activeStore = XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(Session, DifferenceType);
//            if (activeStore != null)
//            {
//                activeStore.active = true;
//                Session.CommitTransaction();
//            }
//        }

//        protected override void OnSaved()
//        {
//            base.OnSaved();
//            XpoModelDictionaryDifferenceStore store = XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(Session, DifferenceType);
//            if (ReferenceEquals(store, this) && !store.Active)
//            {
//                XpoModelDictionaryDifferenceStore activeStore =
//                    GetActiveStores().Take(1).
//                        FirstOrDefault();
//                if (activeStore != null)
//                {
//                    activeStore.active = false;
//                    activeStore.Save();
//                }
//                store.active = true;
//                store.Save();
//                if (Session.InTransaction)
//                    Session.CommitTransaction();
//            }
//        }

//        private IQueryable<XpoModelDictionaryDifferenceStore> GetActiveStores()
//        {
//            return XpoModelDictionaryDifferenceStoreBuilder.GetAll(Session, true, DifferenceType);
//        }


//        public XPCollection<AuditDataItemPersistent> AuditTrail
//        {
//            get
//            {
//                if (auditTrail == null)
//                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
//                return auditTrail;
//            }
//        }

        public bool Disable
        {
            get { return disable; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref disable, value); }
        }

        
        public string Aspect
        {
            get
            {
                return aspect;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref aspect, value);
            }
        }
        [Custom("DisplayFormat", "{0: ddd, dd MMMM yyyy hh:mm:ss tt}")]
        [Custom("EditMask", "ddd, dd MMMM yyyy hh:mm:ss tt")]
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref dateCreated, value); }
        }

        [Size(-1)]
        public string XmlContent
        {
            get
            {
                return xmlContent;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref xmlContent, value);
            }
        }
        [NonPersistent]
        public DictionaryNode Model
        {
            get { return new DictionaryXmlReader().ReadFromString(XmlContent); }
            set
            {
                XmlContent = new DictionaryXmlWriter().GetAspectXml(Aspect, value);
            }
        }
        
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            if (Session.IsNewObject(this))
            {
                aspect = DefaultAspect;
                dateCreated = DateTime.Now;
                Name = "AutoCreated" + DateTime.Now;
                xmlContent = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<Application />";
            }
        }
    }

    public class XpoModelDictionaryDifferenceStoreBuilder
    {
        public const string Caption = "Μοντέλο εφαρμογής";


/*
        private static IQueryable<XpoModelDictionaryDifferenceStore> Query(Session session, DifferenceType differenceType)
        {
            return from xpo in new XPQuery<XpoModelDictionaryDifferenceStore>(session) where xpo.DifferenceType == differenceType select xpo;
        }
*/

        public static IOrderedQueryable<XpoModelDictionaryDifferenceStore> GetActiveStores(bool queryAspect, Session session, DifferenceType differenceType, string applicationTypeName)
        {
            IQueryable<XpoModelDictionaryDifferenceStore> stores =
                from s in new XPQuery<XpoModelDictionaryDifferenceStore>(session)
                where s.Disable == false && s.DifferenceType == differenceType
                      && s.ApplicationTypeName == applicationTypeName
                select s;
            if (queryAspect)
                stores = from s in stores
                         where s.Aspect == XpoModelDictionaryDifferenceStore.DefaultAspect
                         select s;

            return  stores.OrderByDescending(store => store.DateCreated);
        }

        public static XpoModelDictionaryDifferenceStore GetActiveStore(Session session, DifferenceType differenceType, string applicationTypeName)
        {
            return GetActiveStores(true, session, differenceType, applicationTypeName).Take(1).FirstOrDefault();
        }

//        public static IQueryable<XpoModelDictionaryDifferenceStore> GetAll(Session session, bool active, DifferenceType DifferenceType)
//        {
//            return (from store in new XPQuery<XpoModelDictionaryDifferenceStore>(session)
//                    where !(store.Active == false) && store.DifferenceType == DifferenceType
//             select store);
//        }
    }
}