using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpandObjectSpaceProvider : ObjectSpaceProvider, IXpandObjectSpaceProvider {
        readonly bool _dataCache;

        public IXpoDataStoreProxy DataStoreProvider { get; set; }

        public XpandObjectSpaceProvider(IXpoDataStoreProxy provider, bool dataCache)
            : base(provider) {
            _dataCache = dataCache;
            DataStoreProvider = provider;
        }


        protected override IObjectSpace CreateObjectSpaceCore(UnitOfWork unitOfWork, ITypesInfo typesInfo) {
            var objectSpace = new XpandObjectSpace(new XpandUnitOfWork(unitOfWork.DataLayer), typesInfo) {
                AsyncServerModeSourceResolveSession = AsyncServerModeSourceResolveSession,
                AsyncServerModeSourceDismissSession = AsyncServerModeSourceDismissSession
            };
            return objectSpace;
        }

        IObjectSpace IObjectSpaceProvider.CreateUpdatingObjectSpace(Boolean allowUpdateSchema) {
            return CreateObjectSpace();
        }

        private void AsyncServerModeSourceResolveSession(ResolveSessionEventArgs args) {
            IDisposable[] disposableObjects;
            IDataStore dataStore = DataStoreProvider.CreateWorkingStore(out disposableObjects);
            args.Session = CreateUnitOfWork(dataStore, disposableObjects);
        }

        private void AsyncServerModeSourceDismissSession(ResolveSessionEventArgs args) {
            var toDispose = args.Session as IDisposable;
            if (toDispose != null) {
                toDispose.Dispose();
            }
        }
        protected override IDataLayer CreateWorkingDataLayer(IDataStore workingDataStore) {
            if (_dataCache) {
                var cacheRoot = new DataCacheRoot(workingDataStore);
                var cacheNode = new DataCacheNode(cacheRoot);
                return new SimpleDataLayer(XPDictionary, cacheNode);
            }
            return base.CreateWorkingDataLayer(workingDataStore);
        }

        private UnitOfWork CreateUnitOfWork(IDataStore dataStore, IEnumerable<IDisposable> disposableObjects) {
            var disposableObjectsList = new List<IDisposable>();
            if (disposableObjects != null) {
                disposableObjectsList.AddRange(disposableObjects);
            }

            var dataLayer = new SimpleDataLayer(XPDictionary, dataStore);
            disposableObjectsList.Add(dataLayer);
            return new XpandUnitOfWork(dataLayer, disposableObjectsList.ToArray());
        }
    }
}