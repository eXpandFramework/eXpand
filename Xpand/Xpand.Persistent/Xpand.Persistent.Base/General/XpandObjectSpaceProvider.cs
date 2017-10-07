using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General.Model;
using Xpand.Xpo;
using Xpand.Xpo.DB;

namespace Xpand.Persistent.Base.General {
    public class XpandObjectSpaceProvider : XPObjectSpaceProvider, IXpandObjectSpaceProvider, IDatabaseSchemaChecker{
        
        readonly ISecurityStrategyBase _security;
        IDataLayer _dataLayer;
        bool _allowICommandChannelDoWithSecurityContext;
        ClientSideSecurity? _clientSideSecurity;
        public event EventHandler<CreatingWorkingDataLayerArgs> CreatingWorkingDataLayer;
        public new IXpoDataStoreProxy DataStoreProvider { get; set; }

        DatabaseSchemaState IDatabaseSchemaChecker.CheckDatabaseSchemaCompatibility(out Exception exception){
            DatabaseSchemaState result = DatabaseSchemaState.SchemaRequiresUpdate;
            exception = null;
            if (DataStoreProvider != null) {
                try {
                    IDisposable[] disposableObjects;
                    IDataStore dataStore = DataStoreProvider.CreateSchemaCheckingStore(out disposableObjects);
                    if (dataStore != null) {
                        if (dataStore is InMemoryDataStore){
                            var explorer = dataStore as IDataStoreSchemaExplorer;{
                                string[] tablesList = explorer.GetStorageTablesList(true);
                                if (tablesList.Length > 0) {
                                    result = DatabaseSchemaState.SchemaExists;
                                }
                            }
                        }
                        else {
                            using (UnitOfWork unitOfWork = CreateUnitOfWork(dataStore, disposableObjects)) {
                                if (UpdateSchemaResult.SchemaExists == unitOfWork.UpdateSchema(true, GetPersistentClasses())) {
                                    result = DatabaseSchemaState.SchemaExists;
                                }
                            }
                        }
                    }
                }
                catch (UnableToOpenDatabaseException e) {
                    exception = e;
                    result = DatabaseSchemaState.DatabaseMissing;
                }
                catch (SchemaCorrectionNeededException e) {
                    exception = e;
                    result = DatabaseSchemaState.SchemaRequiresUpdate;
                }
                catch (Exception e) {
                    exception = e;
                }
            }
            return result;
        }

        private XPClassInfo[] GetPersistentClasses() {
            ICollection xpoClasses = XPDictionary.Classes;
            List<XPClassInfo> persistentClasses = new List<XPClassInfo>(xpoClasses.Count);
            foreach (XPClassInfo classInfo in xpoClasses) {
                if (classInfo.IsPersistent) {
                    persistentClasses.Add(classInfo);
                }
            }
            var multiDataStoreProxy = DataStoreProvider.Proxy as MultiDataStoreProxy;
            if (multiDataStoreProxy!=null){
                var xpClassInfos = multiDataStoreProxy.DataStoreManager.ReflectionDictionaries[new KeyInfo(false, DataStoreManager.DefaultDictionaryKey)].Classes.OfType<XPClassInfo>().Select(info => info.ClassType).ToArray();
                return persistentClasses.Where(info => xpClassInfos.Contains(info.ClassType)).ToArray();
            }
            return persistentClasses.ToArray();
        }

        public XpandObjectSpaceProvider(IXpoDataStoreProxy provider, ISecurityStrategyBase security, bool threadSafe = false)
            : base(provider, threadSafe) {
            _security = security;
            DataStoreProvider = provider;
            Tracing.Tracer.LogVerboseValue(GetType().FullName,Environment.StackTrace);
        }

        public IDataLayer WorkingDataLayer => _dataLayer;

        string IObjectSpaceProvider.ConnectionString{
            get { return ConnectionString; }
            set{
                var multiDataStoreProvider = new MultiDataStoreProvider(value);
                DataStoreProvider=multiDataStoreProvider;
                SetDataStoreProvider(DataStoreProvider);
            }
        }
        
        public bool AllowICommandChannelDoWithSecurityContext {
            get { return _allowICommandChannelDoWithSecurityContext; }
            set { _allowICommandChannelDoWithSecurityContext = value; }
        }

        protected override IObjectSpace CreateObjectSpaceCore() {
            return new XpandObjectSpace(TypesInfo, XpoTypeInfoSource, CreateUnitOfWork);
        }

        IObjectSpace IObjectSpaceProvider.CreateUpdatingObjectSpace(bool allowUpdateSchema){
            IDisposable[] disposableObjects;
            var dataStore =  DataStoreProvider.CreateUpdatingStore(allowUpdateSchema, out disposableObjects);
            return new XpandObjectSpace(TypesInfo, XpoTypeInfoSource, () => CreateUnitOfWork(dataStore, disposableObjects));
        }

        private XpandUnitOfWork CreateUnitOfWork(IDataStore dataStore, IEnumerable<IDisposable> disposableObjects){
            var disposableObjectsList = new List<IDisposable>();
            if (disposableObjects != null){
                disposableObjectsList.AddRange(disposableObjects);
            }
            var dataLayer = new SimpleDataLayer(XPDictionary, dataStore);
            disposableObjectsList.Add(dataLayer);
            return new XpandUnitOfWork(dataLayer, disposableObjectsList.ToArray());
        }

        public void SetClientSideSecurity(ClientSideSecurity? clientSideSecurity) {
            _clientSideSecurity = clientSideSecurity;
        }

        protected void OnCreatingWorkingDataLayer(CreatingWorkingDataLayerArgs e) {
            EventHandler<CreatingWorkingDataLayerArgs> handler = CreatingWorkingDataLayer;
            handler?.Invoke(this, e);
        }

        protected override IDataLayer CreateDataLayer(IDataStore dataStore) {
            var creatingWorkingDataLayerArgs = new CreatingWorkingDataLayerArgs(dataStore);
            OnCreatingWorkingDataLayer(creatingWorkingDataLayerArgs);
            _dataLayer = creatingWorkingDataLayerArgs.DataLayer ?? CreateDataLayerCore(dataStore);
            return _dataLayer;
        }

        private IDataLayer CreateDataLayerCore(IDataStore dataStore){
            return threadSafe
                ? (IDataLayer) new XpandThreadSafeDataLayer(XPDictionary, dataStore)
                : new SimpleDataLayer(XPDictionary, dataStore);
        }

        private XpandUnitOfWork CreateUnitOfWork() {
            var uow = new XpandUnitOfWork(DataLayer);
            var securedObjectLayer = _security as ISelectDataSecurityProvider;
            if (securedObjectLayer != null &&
                (_clientSideSecurity.HasValue && _clientSideSecurity.Value != ClientSideSecurity.UIlevel)){
                var securityRuleProvider = new SecurityRuleProvider(XPDictionary,securedObjectLayer.CreateSelectDataSecurity());
                var currentObjectLayer = new SecuredSessionObjectLayer(_allowICommandChannelDoWithSecurityContext, uow,true, null, securityRuleProvider, null);
                return new XpandUnitOfWork(currentObjectLayer, uow);
            }
            return uow;
        }
    }


    public class XpandThreadSafeDataLayer : ThreadSafeDataLayer{

        public XpandThreadSafeDataLayer(XPDictionary xpDictionary, IDataStore dataStore) : base(xpDictionary, dataStore){
            
        }

        protected override void OnClassInfoChanged(object sender, ClassInfoEventArgs e){
        }
    }
}