using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpandObjectSpaceProvider : ObjectSpaceProvider, IXpandObjectSpaceProvider {

        public IXpoDataStoreProxy DataStoreProvider { get; set; }

        public XpandObjectSpaceProvider(IXpoDataStoreProxy provider)
            : base(provider) {
            DataStoreProvider = provider;
        }

        protected override IObjectSpace CreateObjectSpaceCore(UnitOfWork unitOfWork, ITypesInfo typesInfo) {
            var objectSpace = new XpandObjectSpace(new XpandUnitOfWork(unitOfWork.DataLayer), typesInfo) {
                AsyncServerModeSourceResolveSession = AsyncServerModeSourceResolveSession,
                AsyncServerModeSourceDismissSession = AsyncServerModeSourceDismissSession
            };
            return objectSpace;
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