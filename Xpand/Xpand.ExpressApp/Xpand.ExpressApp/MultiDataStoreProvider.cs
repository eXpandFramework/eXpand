using DevExpress.Xpo.DB;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp {
    public class MultiDataStoreProvider : DataStoreProvider {
        MultiDataStoreProxy _multiDataStoreProxy;

        public MultiDataStoreProvider(string connectionString)
            : base(connectionString) {
        }

        public MultiDataStoreProvider(IDataStore connectionString)
            : base(connectionString) {
        }

        public override DataStoreProxy Proxy {
            get {
                return ConnectionString != null
                           ? (_multiDataStoreProxy ?? (_multiDataStoreProxy = new MultiDataStoreProxy(ConnectionString)))
                           : base.Proxy;
            }
        }
    }
}