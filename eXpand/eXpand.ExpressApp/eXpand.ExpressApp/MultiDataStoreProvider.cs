using eXpand.Xpo.DB;

namespace eXpand.ExpressApp {
    public class MultiDataStoreProvider:DataStoreProvider {
        XpoMultiDataStoreProxy _xpoMultiDataStoreProxy;

        public MultiDataStoreProvider(string connectionString) : base(connectionString) {
        }

        public override XpoDataStoreProxy Proxy{
            get { return _xpoMultiDataStoreProxy ?? (_xpoMultiDataStoreProxy = new XpoMultiDataStoreProxy(ConnectionString)); }
        }
    }
}