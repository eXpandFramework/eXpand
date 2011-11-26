using System.ServiceModel;
using System.Web;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.DB {

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class XpandDataStoreService : DataStoreService {

        public XpandDataStoreService(string connectionString, bool enableCaching)
            : base(CreateDataStore(connectionString, enableCaching)) {

        }

        static IDataStore CreateDataStore(string connectionString, bool enableCaching) {
            if (!enableCaching) {
                var connectionProvider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
                XpoDefault.Session = null;
                return connectionProvider;
            }
            var dataStore = HttpContext.Current.Application["datastore"] as IDataStore;
            if (dataStore == null) {
                var subjectForCache = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
                var _cacheRoot = new DataCacheRoot(subjectForCache);
                dataStore = new DataCacheNode(_cacheRoot);
                HttpContext.Current.Application["datastore"] = dataStore;
            }
            XpoDefault.Session = null;
            return dataStore;
        }
    }
}
