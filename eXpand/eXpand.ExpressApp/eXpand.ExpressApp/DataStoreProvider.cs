using System;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp
{
    public class DataStoreProvider :  IXpoDataStoreProxy
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
        public virtual IDataStore CreateUpdatingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return Proxy;
        }
        public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return Proxy;
        }
        public XPDictionary XPDictionary
        {
            get { return null; }
        }
        public virtual XpoDataStoreProxy Proxy
        {
            get
            {
                return proxyCore;
            }
        }
    }
}