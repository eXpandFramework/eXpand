using Xpand.Xpo.DB;

namespace Xpand.ExpressApp {
    public class MultiDataStoreProvider:DataStoreProvider {
        SqlMultiDataStoreProxy _sqlMultiDataStoreProxy;

        public MultiDataStoreProvider(string connectionString) : base(connectionString) {
        }

        public override SqlDataStoreProxy Proxy{
            get { return _sqlMultiDataStoreProxy ?? (_sqlMultiDataStoreProxy = new SqlMultiDataStoreProxy(ConnectionString)); }
        }
    }
}