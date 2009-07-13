using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores
{
    public abstract class XpoModelDictionaryDifferenceStoreBase : DevExpress.ExpressApp.DictionaryDifferenceStore
    {
        
        private readonly Session session;
        protected readonly XafApplication application;

        protected XpoModelDictionaryDifferenceStoreBase(Session session, XafApplication application)
        {
            this.session = session;
            this.application = application;
        }


        public Session Session
        {
            get { return session; }
        }

        public override string Name
        {
            get { return DifferenceType.ToString(); }
        }

        public abstract DifferenceType DifferenceType { get; }

        protected override Dictionary LoadDifferenceCore(Schema schema)
        {
            if ((Debugger.IsAttached&&debuggerAttachCheck()) && DifferenceType == DifferenceType.Model)
            {
                DictionaryNode dictionaryNode =
                    new DictionaryXmlReader().ReadFromFile(
                        GetModelPath());
                var differenceCore = new Dictionary(dictionaryNode);
                SaveDifference(differenceCore);
                return differenceCore;
            }
            IOrderedQueryable<BaseObjects.XpoModelDictionaryDifferenceStore> singletonStores = GetActiveStores(false);
            Dictionary dictionary;
            if (singletonStores.Where(store => store.DifferenceType==DifferenceType).Count() == 0)
            {
                dictionary = new Dictionary(new DictionaryNode("Application"), schema);
                SaveDifference(dictionary);
//                return dictionary;
            }

            BaseObjects.XpoModelDictionaryDifferenceStore activeStore = GetActiveStore();

            DictionaryNode rootNode = null;
            if (activeStore != null)
                rootNode = new DictionaryXmlReader().ReadFromString(activeStore.XmlContent);
            if (rootNode == null)
                rootNode = new DictionaryNode("Application");
            dictionary = new Dictionary(rootNode, schema);


            foreach (BaseObjects.XpoModelDictionaryDifferenceStore singletonStore in from store in GetActiveStores(false).Where(store =>activeStore!=
                                                                                                                                        null&& store.Oid!=activeStore.Oid)
//                                                                                     where
//                                                                                         store.Aspect !=
//                                                                                         BaseObjects.XpoModelDictionaryDifferenceStore.DefaultAspect
                                                                                     select store)
                dictionary.AddAspect(singletonStore.Aspect,
                                     new DictionaryXmlReader().ReadFromString(singletonStore.XmlContent));
            return dictionary;
        }

        private bool debuggerAttachCheck()
        {
            string setting = ConfigurationManager.AppSettings["DisableDebuggerAttachedCheck"];
            return string.IsNullOrEmpty(setting) || setting.ToLower() == "false";
        }

        protected virtual string GetModelPath()
        {
            return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Model.xafml");
        }

        protected virtual IOrderedQueryable<BaseObjects.XpoModelDictionaryDifferenceStore> GetActiveStores(
            bool queryAspect)
        {
            return XpoModelDictionaryDifferenceStoreBuilder.GetActiveStores(queryAspect, session, DifferenceType, application.GetType().FullName);
        }

        public override void SaveDifference(Dictionary diffDictionary)
        {
            if (diffDictionary != null)
                foreach (string aspect in diffDictionary.Aspects)
                {
                    BaseObjects.XpoModelDictionaryDifferenceStore dictionaryDifferenceStore = GetActiveStore() ??getNewStore(session);
                    BaseObjects.XpoModelDictionaryDifferenceStore xpoUserModelDictionaryDifferenceStore =dictionaryDifferenceStore;
                    xpoUserModelDictionaryDifferenceStore.Aspect = aspect;
                    xpoUserModelDictionaryDifferenceStore.DifferenceType = DifferenceType;
                    xpoUserModelDictionaryDifferenceStore.ApplicationTypeName = application.GetType().FullName;


                    xpoUserModelDictionaryDifferenceStore.XmlContent =
                        "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" +
                        (new DictionaryXmlWriter()).GetAspectXml(DictionaryAttribute.DefaultLanguage,
                                                                 diffDictionary.RootNode);

                    OnXpoUserModelDictionaryDifferenceStoreSaving(xpoUserModelDictionaryDifferenceStore);
                }
            
        }

        protected virtual bool AllowCustomPersistence
        {
            get { return true; }
        }

        protected virtual BaseObjects.XpoModelDictionaryDifferenceStore GetActiveStore()
        {
            return XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(Session, DifferenceType, application.GetType().FullName);
        }

        protected abstract BaseObjects.XpoModelDictionaryDifferenceStore getNewStore(Session session);

        protected virtual void OnXpoUserModelDictionaryDifferenceStoreSaving(
            BaseObjects.XpoModelDictionaryDifferenceStore xpoUserModelDictionaryDifferenceStore)
        {
            xpoUserModelDictionaryDifferenceStore.Save();
            if (session is UnitOfWork)
                ((UnitOfWork) session).CommitChanges();
        }
    }
}