using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Core {
    public static class XafApplicationExtensions {

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            ((ISupportFullConnectionString) xafApplication).ConnectionString =getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
            args.ObjectSpaceProvider = new XpandObjectSpaceProvider(new MultiDataStoreProvider(args.ConnectionString));
        }

        static string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }

    }
}