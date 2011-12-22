using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo.DB;

namespace Xpand.ExpressApp.MiddleTier {
    public class XpandServerApplication : ServerApplication,ISupportFullConnectionString,IXafApplication {
        string ISupportFullConnectionString.ConnectionString { get; set; }
        public DataCacheNode GetDataCacheRoot(IDataStore dataStore) {
            return null;
        }
    }
}
