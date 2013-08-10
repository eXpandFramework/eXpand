using System;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.DB;

namespace Xpand.Persistent.Base.General {
    public class DataStoreProvider : IXpoDataStoreProxy {
        private readonly DataStoreProxy _storeProxy;
        private readonly string _connectionString;
        public DataStoreProvider(string connectionString) {
            _connectionString = connectionString;
            _storeProxy = new DataStoreProxy(connectionString);
        }

        public DataStoreProvider(IDataStore dataStore) {
            _storeProxy = new DataStoreProxy(dataStore);
            var connectionProviderSql = dataStore as ConnectionProviderSql;
            if (connectionProviderSql!=null)
                _connectionString = connectionProviderSql.ConnectionString;
        }

        public string ConnectionString {
            get { return _connectionString; }
        }
        public virtual IDataStore CreateUpdatingStore(out IDisposable[] disposableObjects) {
            disposableObjects = null;
            return Proxy;
        }
        public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects) {
            disposableObjects = null;
            return Proxy;
        }
        public XPDictionary XPDictionary {
            get { return null; }
        }
        public virtual DataStoreProxy Proxy {
            get {
                return _storeProxy;
            }
        }
    }
}