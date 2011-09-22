using System;
using System.Configuration;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Core {
    public static class XafApplicationExtensions {

        public static void CheckObjectSpaceProviderType<T>(this XafApplication xafApplication) where T : IObjectSpaceProvider {
            CheckObjectSpaceProviderType<T>(xafApplication, true);
        }

        public static T FindModule<T>(this XafApplication xafApplication) where T : ModuleBase {
            return (T)xafApplication.Modules.FindModule(typeof(T));
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            ((ISupportFullConnectionString)xafApplication).ConnectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
            string setting = ConfigurationManager.AppSettings["SqlProxy"];
            if (!string.IsNullOrEmpty(setting) && bool.Parse(setting))
                args.ObjectSpaceProvider = new XpandObjectSpaceProvider(new MultiDataStoreProvider(args.ConnectionString));
        }

        static string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }

        public static bool CheckObjectSpaceProviderType<T>(this XafApplication xafApplication, bool throwEx) {
            if (xafApplication != null && xafApplication.ObjectSpaceProvider != null) {
                bool b = (xafApplication.ObjectSpaceProvider is T);
                if (!b && throwEx) {
                    string fullName = "ObjectSpaceProvider";
                    if (xafApplication.ObjectSpaceProvider != null)
                        fullName = xafApplication.ObjectSpaceProvider.GetType().FullName;
                    throw new NotImplementedException(fullName + " should implement " + typeof(T).FullName + " interface. Consider setting the SqlProxy key value to true in your config file");
                }
                return b;
            }
            return true;
        }
    }
}