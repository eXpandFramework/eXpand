using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo.DB;

namespace Xpand.Persistent.Base.General {
    public class MultiDataStoreProvider : DataStoreProvider {
        MultiDataStoreProxy _multiDataStoreProxy;

        public MultiDataStoreProvider(string connectionString)
            : base(connectionString) {
        }

        public MultiDataStoreProvider(IDataStore connectionString)
            : base(connectionString) {
        }

        public override IDataStore CreateUpdatingStore(out IDisposable[] disposableObjects){
            disposableObjects = new IDisposable[]{};
            return new MultiDataStoreProxy(XpoDefault.GetConnectionProvider(ConnectionString,AutoCreateOption.DatabaseAndSchema), ConnectionString);
        }

        public override DataStoreProxy Proxy {
            get {
                return ConnectionString != null
                           ? (_multiDataStoreProxy ?? (_multiDataStoreProxy = new MultiDataStoreProxy(XpoDefault.GetConnectionProvider(ConnectionString,AutoCreateOption.None), ConnectionString)))
                           : base.Proxy;
            }
        }
    }
}