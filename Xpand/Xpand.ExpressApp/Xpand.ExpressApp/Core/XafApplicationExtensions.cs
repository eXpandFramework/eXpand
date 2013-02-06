using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using Xpand.ExpressApp.MiddleTier;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.Core {
    public static class XafApplicationExtensions {
        public static T FindModule<T>(this XafApplication xafApplication) where T : ModuleBase {
            return (T)xafApplication.Modules.FindModule(typeof(T));
        }

        public static int DropDatabaseOnVersionMissmatch(this XafApplication xafApplication) {
            int missMatchCount = 0;
            if (VersionMissMatch(xafApplication)) {
                foreach (ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings) {
                    try {
                        DropSqlServerDatabase(settings.ConnectionString);
                        missMatchCount++;
                    } catch (UnableToOpenDatabaseException) {
                    } catch (Exception e) {
                        Tracing.Tracer.LogError(e);
                    }
                }
            }
            return missMatchCount;
        }

        static bool VersionMissMatch(XafApplication xafApplication) {
            var assemblyVersion = ReflectionHelper.GetAssemblyVersion(typeof(XpandSystemModule).Assembly);
            var xpandSystemModule = xafApplication.Modules.FindModule<XpandSystemModule>();
            return xpandSystemModule != null && xpandSystemModule.Version != assemblyVersion;
        }

        private static void DropSqlServerDatabase(string connectionString) {
            var connectionProvider = (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionString, DevExpress.Xpo.DB.AutoCreateOption.None);
            using (var dbConnection = connectionProvider.Connection) {
                using (var sqlConnection = (SqlConnection)DataStore(connectionString).Connection) {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbConnection.Database);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = string.Format("DROP DATABASE {0}", dbConnection.Database);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        static MSSqlConnectionProvider DataStore(string connectionString) {
            var connectionStringParser = new ConnectionStringParser(connectionString);
            var userid = connectionStringParser.GetPartByName("UserId");
            var password = connectionStringParser.GetPartByName("password");
            string connectionStr;
            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(password))
                connectionStr = MSSqlConnectionProvider.GetConnectionString(connectionStringParser.GetPartByName("Data Source"), userid, password, "master");
            else {
                connectionStr = MSSqlConnectionProvider.GetConnectionString(connectionStringParser.GetPartByName("Data Source"), "master");
            }
            return (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionStr, DevExpress.Xpo.DB.AutoCreateOption.None);
        }

        public static ClientSideSecurity? ClientSideSecurity(this XafApplication xafApplication) {
            var modelOptionsClientSideSecurity = xafApplication.Model.Options as IModelOptionsClientSideSecurity;
            return modelOptionsClientSideSecurity != null ? (modelOptionsClientSideSecurity).ClientSideSecurity : null;
        }

        public static SimpleDataLayer CreateCachedDataLayer(this XafApplication xafApplication, IDataStore argsDataStore) {
            var cacheRoot = new DataCacheRoot(argsDataStore);
            var cacheNode = new DataCacheNode(cacheRoot);
            return new SimpleDataLayer(XpandModuleBase.Dictiorary, cacheNode);
        }

        public static string GetConnectionString(this XafApplication xafApplication) {
            if (xafApplication is ServerApplication && !(xafApplication is IXafApplication))
                throw new NotImplementedException("Use " + typeof(XpandServerApplication) + " insted of " + xafApplication.GetType());
            return ((IConnectionString)xafApplication).ConnectionString;
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            var connectionString = ConnectionString(xafApplication, args);
            var connectionProvider = XpoDefault.GetConnectionProvider(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            args.ObjectSpaceProvider = ObjectSpaceProvider(xafApplication, connectionProvider, connectionString);
        }

        static string ConnectionString(XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            var connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
            ((IConnectionString)xafApplication).ConnectionString = connectionString;
            return connectionString;
        }

        public static AutoCreateOption AutoCreateOption(this XafApplication xafApplication) {
            return ConfigurationManager.AppSettings.AllKeys.Contains("AutoCreateOption")
                       ? (AutoCreateOption)
                         Enum.Parse(typeof(AutoCreateOption), ConfigurationManager.AppSettings["AutoCreateOption"])
                       : DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema;
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args, string dataStoreNameSuffix) {
            if (dataStoreNameSuffix == null) {
                var connectionString = ConnectionString(xafApplication, args);
                var connectionProvider = XpoDefault.GetConnectionProvider(connectionString, ((IXafApplication)xafApplication).AutoCreateOption);
                args.ObjectSpaceProvider = new XPObjectSpaceProvider(new DataStoreProvider(connectionProvider));
            } else if (DataStoreManager.GetDataStoreAttributes(dataStoreNameSuffix).Any()) {
                xafApplication.CreateCustomObjectSpaceprovider(args);
            }
        }

        static IObjectSpaceProvider ObjectSpaceProvider(XafApplication xafApplication, IDataStore connectionProvider, string connectionString) {
            var xafApplicationDataStore = xafApplication as IXafApplicationDataStore;
            var selectDataSecurityProvider = xafApplication.Security as ISelectDataSecurityProvider;
            if (xafApplicationDataStore != null) {
                IDataStore dataStore = xafApplicationDataStore.GetDataStore(connectionProvider);
                return dataStore != null ? new XpandObjectSpaceProvider(new MultiDataStoreProvider(dataStore), selectDataSecurityProvider)
                                          : new XpandObjectSpaceProvider(new MultiDataStoreProvider(connectionString), selectDataSecurityProvider);
            }
            return new XpandObjectSpaceProvider(new MultiDataStoreProvider(connectionString), selectDataSecurityProvider);
        }

        static string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}