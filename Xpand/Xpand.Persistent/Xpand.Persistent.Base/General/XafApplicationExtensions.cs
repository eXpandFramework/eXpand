using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Persistent.Base.General.Model;
using Xpand.Xpo.DB;

namespace Xpand.Persistent.Base.General {
    public static class XafApplicationExtensions {
        static  XafApplicationExtensions() {
            DisableObjectSpaceProderCreation = true;
        }

        public static ReadOnlyCollection<Controller> ActualControllers(this ControllersManager controllersManager){
            return (ReadOnlyCollection<Controller>) controllersManager.GetPropertyValue("ActualControllers");
        }

        public static void EnsureShowViewStrategy(this XafApplication xafApplication){
            xafApplication.CallMethod("EnsureShowViewStrategy");
        }

        public static XPDictionary GetXPDictionary(this XafApplication xafApplication){
            return XpandModuleBase.Dictiorary;
        }

        public static bool CanBuildSecurityObjects(this XafApplication xafApplication) {
            return (xafApplication.Security != null && xafApplication.Security.UserType != null && !xafApplication.Security.UserType.IsInterface);
        }

        public static View CreateView(this XafApplication application, IModelView viewModel) {
            return (View) application.CallMethod("CreateView", viewModel);
        }

        public static Controller CreateController(this XafApplication application,Type type) {
            var methodInfo = typeof (XafApplication).Method(new[]{type}, "CreateController", Flags.InstancePublicDeclaredOnly);
            return (Controller) methodInfo.Call(application);
        }

        public static T FindModule<T>(this XafApplication xafApplication,bool extactMatch=true) where T : ModuleBase{
            var moduleType = typeof(T);
            if (moduleType.IsInterface || moduleType.IsAbstract)
                extactMatch = false;
            return !extactMatch
                ? (T) xafApplication.Modules.FirstOrDefault(@base => @base is T)
                : (T) xafApplication.Modules.FindModule(moduleType);
        }

        public static void SetClientSideSecurity(this XafApplication xafApplication) {
            var xpandObjectSpaceProvider = (xafApplication.ObjectSpaceProvider as XpandObjectSpaceProvider);
            if (xpandObjectSpaceProvider != null)
                xpandObjectSpaceProvider.SetClientSideSecurity(xafApplication.ClientSideSecurity());
            else {
                var modelOptionsClientSideSecurity = xafApplication.Model.Options as IModelOptionsClientSideSecurity;
                if (modelOptionsClientSideSecurity != null && modelOptionsClientSideSecurity.ClientSideSecurity != null &&
                    modelOptionsClientSideSecurity.ClientSideSecurity.Value == Model.ClientSideSecurity.IntegratedMode) {
                    throw new Exception("Set Application.Model.Options.ClientSideSecurity to another value than IntegratedMode or use " + typeof(XpandObjectSpaceProvider).FullName);
                }
            }
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
            var assemblyVersion = ReflectionHelper.GetAssemblyVersion(typeof(XafApplicationExtensions).Assembly);
            var xpandModule = xafApplication.Modules.First(@base => @base is XpandModuleBase);
            return xpandModule != null && xpandModule.Version != assemblyVersion;
        }

        private static void DropSqlServerDatabase(string connectionString) {
            var connectionProvider = (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None);
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
            return (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionStr, AutoCreateOption.None);
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

        public static bool DisableObjectSpaceProderCreation { get; set; }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            if (DisableObjectSpaceProderCreation)
                return;
            var connectionString = ConnectionString(xafApplication, args);
            args.ObjectSpaceProviders.Add(ObjectSpaceProvider(xafApplication,  connectionString));
        }

        static string ConnectionString(XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            var connectionString = GetConnectionStringWithOutThreadSafeDataLayerInitialization(args);
            if (!xafApplication.ObjectSpaceProviders.Any())
                xafApplication.ConnectionString = connectionString;
            return connectionString;
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args, string dataStoreName) {
            if (dataStoreName == null) {
                var connectionString = ConnectionString(xafApplication, args);
                args.ObjectSpaceProvider = ObjectSpaceProvider(xafApplication,  connectionString);
            } else if (DataStoreManager.GetDataStoreAttributes(dataStoreName).Any()) {
                var disableObjectSpaceProderCreation = DisableObjectSpaceProderCreation;
                DisableObjectSpaceProderCreation = false;
                xafApplication.CreateCustomObjectSpaceprovider(args);
                DisableObjectSpaceProderCreation=disableObjectSpaceProderCreation;
            }
        }

        static IObjectSpaceProvider ObjectSpaceProvider(XafApplication xafApplication,  string connectionString) {
            return new XpandObjectSpaceProvider(new MultiDataStoreProvider(connectionString), xafApplication.Security);
        }

        static string GetConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}