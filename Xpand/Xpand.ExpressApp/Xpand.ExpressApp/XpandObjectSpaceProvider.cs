using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;
using System.Reflection;

namespace Xpand.ExpressApp
{
    public class XpandObjectSpaceProvider : DevExpress.ExpressApp.ObjectSpaceProvider, IXpandObjectSpaceProvider {
        
        public IXpoDataStoreProxy DataStoreProvider { get; set; }

        public XpandObjectSpaceProvider(IXpoDataStoreProxy provider)
            : base(provider)
        {
            DataStoreProvider = provider;
        }

        protected override IObjectSpace CreateObjectSpaceCore(UnitOfWork unitOfWork, ITypesInfo typesInfo)
        {
            var objectSpace = new XpandObjectSpace(new XpandUnitOfWork(unitOfWork.DataLayer), typesInfo);
            objectSpace.GetType().BaseType.GetField(
                "AsyncServerModeSourceResolveSession", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(objectSpace, new Action<ResolveSessionEventArgs>(AsyncServerModeSourceResolveSession));

            objectSpace.GetType().BaseType.GetField(
                "AsyncServerModeSourceDismissSession", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(objectSpace, new Action<ResolveSessionEventArgs>(AsyncServerModeSourceDismissSession));

            return objectSpace;
        }

        private void AsyncServerModeSourceResolveSession(ResolveSessionEventArgs args)
        {
            IDisposable[] disposableObjects = null;
            IDataStore dataStore = DataStoreProvider.CreateWorkingStore(out disposableObjects);
            args.Session = this.CreateUnitOfWork(dataStore, disposableObjects);
        }

        private void AsyncServerModeSourceDismissSession(ResolveSessionEventArgs args)
        {
            IDisposable toDispose = args.Session as IDisposable;
            if (toDispose != null)
            {
                toDispose.Dispose();
            }
        }

        private UnitOfWork CreateUnitOfWork(IDataStore dataStore, IDisposable[] disposableObjects)
        {
            List<IDisposable> disposableObjectsList = new List<IDisposable>();
            if (disposableObjects != null)
            {
                disposableObjectsList.AddRange(disposableObjects);
            }

            var dataLayer = new SimpleDataLayer(XPDictionary, dataStore);
            disposableObjectsList.Add(dataLayer);
            return new XpandUnitOfWork(dataLayer, disposableObjectsList.ToArray());
        }
    }
}