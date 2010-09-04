using System;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp
{
    public class DataStoreProvider :  IXpoDataStoreProxy
    {
        private readonly SqlDataStoreProxy proxyCore;
        private readonly string connectionStringCore;
        public DataStoreProvider(string connectionString)
        {
            connectionStringCore = connectionString;
            proxyCore = new SqlDataStoreProxy(connectionString);
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
        public virtual SqlDataStoreProxy Proxy
        {
            get
            {
                return proxyCore;
            }
        }
    }
}