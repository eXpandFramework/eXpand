using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.MiddleTier {
    public class XpandServerApplication : ServerApplication, ISupportFullConnectionString, IXafApplication, ISupportModelsManager {
        string ISupportFullConnectionString.ConnectionString { get; set; }

        IDataStore IXafApplication.GetDataStore(IDataStore dataStore) {
            return null;
        }


        public new string ConnectionString {
            get { return base.ConnectionString; }
            set {
                base.ConnectionString = value;
                ((ISupportFullConnectionString)this).ConnectionString = value;
            }
        }

        string IXafApplication.RaiseEstablishingConnection() {
            return this.GetConnectionString();
        }

        public ApplicationModelsManager ModelsManager {
            get { return modelsManager; }
        }
    }
}
