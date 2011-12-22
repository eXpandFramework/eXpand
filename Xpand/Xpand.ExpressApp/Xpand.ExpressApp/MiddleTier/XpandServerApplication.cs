using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.MiddleTier {
    public class XpandServerApplication : ServerApplication,ISupportFullConnectionString,IXafApplication {
        string ISupportFullConnectionString.ConnectionString { get; set; }

        IDataStore IXafApplication.GetDataStore(IDataStore dataStore) {
            return null;
        }

        string IXafApplication.RaiseEstablishingConnection() {
            return this.GetConnectionString();
        }
    }
}
