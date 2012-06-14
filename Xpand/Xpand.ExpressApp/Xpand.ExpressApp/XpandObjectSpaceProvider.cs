using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpandObjectSpaceProvider : XPObjectSpaceProvider, IXpandObjectSpaceProvider {
        readonly ISelectDataSecurityProvider _selectDataSecurityProvider;
        IDataLayer _dataLayer;
        bool _allowICommandChannelDoWithSecurityContext;


        public new IXpoDataStoreProxy DataStoreProvider { get; set; }

        public XpandObjectSpaceProvider(IXpoDataStoreProxy provider, ISelectDataSecurityProvider selectDataSecurityProvider)
            : base(provider) {
            _selectDataSecurityProvider = selectDataSecurityProvider;
            DataStoreProvider = provider;
        }

        public ISelectDataSecurityProvider SelectDataSecurityProvider {
            get { return _selectDataSecurityProvider; }
        }

        public new IDataLayer WorkingDataLayer {
            get { return _dataLayer; }
        }

        public bool AllowICommandChannelDoWithSecurityContext {
            get { return _allowICommandChannelDoWithSecurityContext; }
            set { _allowICommandChannelDoWithSecurityContext = value; }
        }

        protected override IObjectSpace CreateObjectSpaceCore() {
            IDisposable[] disposableObjects;
            IDataStore dataStore = DataStoreProvider.CreateWorkingStore(out disposableObjects);
            return new XpandObjectSpace(TypesInfo, XpoTypeInfoSource, () => CreateUnitOfWork(dataStore, disposableObjects));
        }

        IObjectSpace IObjectSpaceProvider.CreateUpdatingObjectSpace(Boolean allowUpdateSchema) {
            return CreateObjectSpace();
        }

        public event EventHandler<CreatingWorkingDataLayerArgs> CreatingWorkingDataLayer;

        protected void OnCreatingWorkingDataLayer(CreatingWorkingDataLayerArgs e) {
            EventHandler<CreatingWorkingDataLayerArgs> handler = CreatingWorkingDataLayer;
            if (handler != null) handler(this, e);
        }
        protected override IDataLayer CreateDataLayer(IDataStore dataStore) {
            var creatingWorkingDataLayerArgs = new CreatingWorkingDataLayerArgs(dataStore);
            OnCreatingWorkingDataLayer(creatingWorkingDataLayerArgs);
            _dataLayer = creatingWorkingDataLayerArgs.DataLayer ?? base.CreateDataLayer(dataStore);
            return _dataLayer;
        }

        private XpandUnitOfWork CreateUnitOfWork(IDataStore dataStore, IEnumerable<IDisposable> disposableObjects) {
            var disposableObjectsList = new List<IDisposable>();
            if (disposableObjects != null) {
                disposableObjectsList.AddRange(disposableObjects);
            }

            var dataLayer = new SimpleDataLayer(XPDictionary, dataStore);
            disposableObjectsList.Add(dataLayer);
            if (SelectDataSecurityProvider==null )
                return new XpandUnitOfWork(dataLayer, disposableObjectsList.ToArray());
            var xpandUnitOfWork = new XpandUnitOfWork(dataLayer, disposableObjectsList.ToArray());
            SessionObjectLayer currentObjectLayer = new SecuredSessionObjectLayer(_allowICommandChannelDoWithSecurityContext, xpandUnitOfWork, true, null, new SecurityRuleProvider(XPDictionary, _selectDataSecurityProvider.CreateSelectDataSecurity()), null);
            return new XpandUnitOfWork(currentObjectLayer, xpandUnitOfWork);
        }
    }

    public class CreatingWorkingDataLayerArgs : EventArgs {
        readonly IDataStore _workingDataStore;

        public CreatingWorkingDataLayerArgs(IDataStore workingDataStore) {
            _workingDataStore = workingDataStore;
        }

        public IDataStore WorkingDataStore {
            get { return _workingDataStore; }
        }

        public IDataLayer DataLayer { get; set; }
    }
}