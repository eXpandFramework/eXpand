using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp
{

    public class DataStoreProvider : IXpoDataStoreProvider
    {
        private readonly XpoDataStoreProxy proxyCore;
        private readonly string connectionStringCore;
        public DataStoreProvider(string connectionString)
        {
            connectionStringCore = connectionString;
            proxyCore = new XpoDataStoreProxy(connectionString);
        }
        public string ConnectionString
        {
            get { return connectionStringCore; }
        }
        public DevExpress.Xpo.DB.IDataStore CreateUpdatingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return proxyCore;
        }
        public DevExpress.Xpo.DB.IDataStore CreateWorkingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return proxyCore;
        }
        public XPDictionary XPDictionary
        {
            get { return null; }
        }
        public XpoDataStoreProxy Proxy
        {
            get
            {
                return proxyCore;
            }
        }
    }
}