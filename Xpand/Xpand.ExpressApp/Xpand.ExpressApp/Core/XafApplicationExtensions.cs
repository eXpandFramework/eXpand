using System.Configuration;
using DevExpress.ExpressApp;
using Xpand.Xpo.DB;
using System.Linq;

namespace Xpand.ExpressApp.Core {
    public static class XafApplicationExtensions {
        public static T FindModule<T>(this XafApplication xafApplication) where T : ModuleBase {
            return (T)xafApplication.Modules.FindModule(typeof(T));
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            ((ISupportFullConnectionString)xafApplication).ConnectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
            args.ObjectSpaceProvider = new XpandObjectSpaceProvider(new MultiDataStoreProvider(args.ConnectionString), ConfigurationManager.AppSettings["DataCache"] == "Client");
        }

        static string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}