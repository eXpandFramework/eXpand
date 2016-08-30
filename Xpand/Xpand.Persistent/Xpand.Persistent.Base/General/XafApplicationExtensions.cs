using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Persistent.Base.General.Model;
using Xpand.Xpo.DB;
using FileLocation = Xpand.Persistent.Base.ModelAdapter.FileLocation;

namespace Xpand.Persistent.Base.General {
    public static class XafApplicationExtensions {
        static  XafApplicationExtensions() {
            DisableObjectSpaceProderCreation = true;
        }

        public static ListView CreateListView<T>(this XafApplication application, IObjectSpace objectSpace,bool isRoot=true){
            var objectType = objectSpace.TypesInfo.FindBussinessObjectType<T>();
            return application.CreateListView(objectSpace, objectType, isRoot);
        }

        public static void AddObjectSpaceProvider(this XafApplication application, IObjectSpaceProvider objectSpaceProvider){
            var objectSpaceProviders = ((List<IObjectSpaceProvider>)application.GetFieldValue("objectSpaceProviders"));
            objectSpaceProviders.Add(objectSpaceProvider);
        }

        public static IObjectSpace CreateObjectSpace<T>(this XafApplication application){
            return application.CreateObjectSpace(application.TypesInfo.FindBussinessObjectType<T>());
        }

        public static bool IsHosted(this XafApplication application){
            return application.Modules.AreHosted();
        }

        internal static bool AreHosted(this IEnumerable<ModuleBase> moduleBases) {
            var modules = moduleBases as ModuleBase[] ?? moduleBases.ToArray();
            var hosted = modules.Any(@base =>{
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(@base.GetType());
                var attribute = typeInfo.FindAttribute<ToolboxItemFilterAttribute>();
                return attribute != null && attribute.FilterString == "Xaf.Platform.Web";
            });
            if (hosted){
                if (!modules.Any(@base =>{
                    var typeInfo = XafTypesInfo.Instance.FindTypeInfo(@base.GetType());
                    var attribute = typeInfo.FindAttribute<ToolboxItemFilterAttribute>();
                    return attribute != null && attribute.FilterString == "Xaf.Platform.Win";
                })){
                    return true;
                }
                throw new NotSupportedException("Cannot load modules from different platforms");
            }
            return false;
        }

        public static string GetStorageFolder(this XafApplication app,string folderName){
            var fileLocation = GetFileLocation(FileLocation.ApplicationFolder,folderName);
            switch (fileLocation){
                case FileLocation.CurrentUserApplicationDataFolder:
                    return System.Windows.Forms.Application.UserAppDataPath;
                default:
                    return PathHelper.GetApplicationFolder();
            }            
        }

        static T GetFileLocation<T>(T defaultValue, string keyName) {
            T result = defaultValue;
            string value = ConfigurationManager.AppSettings[keyName];
            if (!string.IsNullOrEmpty(value)) {
                result = (T)Enum.Parse(typeof(T), value, true);
            }
            return result;
        }

        public static bool GetEasyTestParameter(this XafApplication app,string parameter){
            if (app!=null){
                var paramFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "", "easytestparameters");
                return File.Exists(paramFile) && File.ReadAllLines(paramFile).Any(s => s == parameter);
            }
            return false;
        }

        public static IEnumerable<Controller> CreateValidationControllers(this XafApplication app){
            yield return app.CreateController<ActionValidationController>();
            yield return app.CreateController<PersistenceValidationController>();
            yield return app.CreateController<ResultsHighlightController>();
            yield return app.CreateController<RuleSetInitializationController>();
        }

        public static IEnumerable<Controller> CreateAppearenceControllers(this XafApplication app){
            yield return app.CreateController<ActionAppearanceController>();
            yield return app.CreateController<AppearanceController>();
            yield return app.CreateController<DetailViewItemAppearanceController>();
            yield return app.CreateController<DetailViewLayoutItemAppearanceController>();
            yield return app.CreateController<RefreshAppearanceController>();
            yield return app.CreateController<AppearanceCustomizationListenerController>();
        }

        public static bool IsLoggedIn(this XafApplication application){
            return SecuritySystem.CurrentUser != null;
        }

        public static void WriteLastLogonParameters(this XafApplication application,DetailView detailView=null){
            var parameterTypes = new[] { typeof(DetailView), typeof(object) };
            application.CallMethod("WriteLastLogonParameters", parameterTypes, detailView, SecuritySystem.LogonParameters);
        }

        public static void ReadLastLogonParameters(this XafApplication application) {
            application.CallMethod("ReadLastLogonParameters", SecuritySystem.LogonParameters);
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
            return (xafApplication.Security?.UserType != null && !xafApplication.Security.UserType.IsInterface);
        }

        public static View CreateView(this XafApplication application, IModelView viewModel) {
            return (View) application.CallMethod("CreateView", viewModel);
        }

        public static Controller CreateController(this XafApplication application,Type type){
            return (Controller) application.CallMethod(new[]{type}, "CreateController");
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
            var xpandObjectSpaceProvider = (xafApplication.ObjectSpaceProviders.OfType<XpandObjectSpaceProvider>().FirstOrDefault());
            if (xpandObjectSpaceProvider != null)
                xpandObjectSpaceProvider.SetClientSideSecurity(xafApplication.ClientSideSecurity());
            else {
                var modelOptionsClientSideSecurity = xafApplication.Model.Options as IModelOptionsClientSideSecurity;
                if (modelOptionsClientSideSecurity?.ClientSideSecurity != null && modelOptionsClientSideSecurity.ClientSideSecurity.Value == Model.ClientSideSecurity.IntegratedMode) {
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
                    sqlCommand.CommandText =
                        $"ALTER DATABASE {dbConnection.Database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = $"DROP DATABASE {dbConnection.Database}";
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
            return modelOptionsClientSideSecurity?.ClientSideSecurity;
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
            return new XpandObjectSpaceProvider(new MultiDataStoreProvider(connectionString), xafApplication.Security,xafApplication.IsHosted());
        }

        static string GetConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}