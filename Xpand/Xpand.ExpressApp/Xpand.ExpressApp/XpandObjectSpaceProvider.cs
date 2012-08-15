using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Model;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpandObjectSpaceProvider : XPObjectSpaceProvider, IXpandObjectSpaceProvider {
        readonly ISelectDataSecurityProvider _selectDataSecurityProvider;
        IDataLayer _dataLayer;
        bool _allowICommandChannelDoWithSecurityContext;
        ClientSideSecurity? _clientSideSecurity;


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
            return new XpandObjectSpace(TypesInfo, XpoTypeInfoSource, CreateUnitOfWork);
        }

        IObjectSpace IObjectSpaceProvider.CreateUpdatingObjectSpace(Boolean allowUpdateSchema) {
            return CreateObjectSpace();
        }

        public void SetClientSideSecurity(ClientSideSecurity? clientSideSecurity) {
            _clientSideSecurity = clientSideSecurity;
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

        private XpandUnitOfWork CreateUnitOfWork() {
            var uow = new XpandUnitOfWork(DataLayer);

            if (SelectDataSecurityProvider == null)
                return uow;
            if (!_clientSideSecurity.HasValue || _clientSideSecurity.Value == ClientSideSecurity.UIlevel)
                return uow;
            var currentObjectLayer = new SecuredSessionObjectLayer(_allowICommandChannelDoWithSecurityContext, uow, true, null, new SecurityRuleProvider(XPDictionary, _selectDataSecurityProvider.CreateSelectDataSecurity()), null);
            return new XpandUnitOfWork(currentObjectLayer, uow);
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