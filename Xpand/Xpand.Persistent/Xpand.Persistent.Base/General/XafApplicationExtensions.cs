using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using Fasterflect;
using Xpand.Extensions.AppDomainExtensions;
using Xpand.Extensions.ProcessExtensions;
using Xpand.Extensions.StreamExtensions;
using Xpand.Persistent.Base.General.Model;
using Xpand.Xpo.DB;
using DeviceCategory = Xpand.Persistent.Base.ModelDifference.DeviceCategory;
using FileLocation = Xpand.Persistent.Base.ModelAdapter.FileLocation;

namespace Xpand.Persistent.Base.General {

    public enum Platform{
        Agnostic,Win, Web,Mobile,
    }
    public static class XafApplicationExtensions {

        static  XafApplicationExtensions() {
            DisableObjectSpaceProviderCreation = true;
        }
        private static readonly object Locker=new();
        

        public static void ShowView(this XafApplication application, View view) {
            application.ShowViewStrategy.ShowView(new ShowViewParameters(view),new ShowViewSource(application.MainWindow,null) );
        }

        public static Task<int> ShowToastAsync(this XafApplication application, string text=null) {
            lock (Locker) {
                var path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\WindowsPowerShell\Modules\BurntToast";
                bool needsCleanup = false;
                if (!Directory.Exists(path)) {
                    needsCleanup = true;
                    Directory.CreateDirectory(path);
                    
                    var stream = typeof(XafApplicationExtensions).Assembly.GetManifestResourceStream("Xpand.Persistent.Base.Resources.BurntToast.psm1");
                    stream.SaveToFile(Path.Combine(path,"BurntToast.psm1"));
                    stream = typeof(XafApplicationExtensions).Assembly.GetManifestResourceStream("Xpand.Persistent.Base.Resources.config.json");
                    stream.SaveToFile(Path.Combine(path,"config.json"));
                    stream = typeof(XafApplicationExtensions).Assembly.GetManifestResourceStream("Xpand.Persistent.Base.Resources.Microsoft.Toolkit.Uwp.Notifications.dll");
                    stream.SaveToFile(Path.Combine($@"{path}\lib\Microsoft.Toolkit.Uwp.Notifications\","Microsoft.Toolkit.Uwp.Notifications.dll"));
                }
                var processStartInfo =
                    new ProcessStartInfo("powershell", $@"-command ""& {{ &'New-BurntToastNotification' -Text ""{text}}}""") {
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                var process = new Process {StartInfo = processStartInfo};
                return process.RunProcessAsync().ContinueWith(task => {
                    if (needsCleanup)
                        Directory.Delete(path,true);
                    return task.Result;
                });
                
            }
        }

        public static void SendMail(this string body,string subject=null,bool isBodyHtml=false) {
            subject ??= ApplicationHelper.Instance.Application.Title;
            using var smtpClient = new SmtpClient();
            var appSettings = ConfigurationManager.AppSettings;
            var errorMailRecipients = appSettings["ErrorMailReceipients"];
            if (errorMailRecipients == null)
                throw new NullReferenceException("Configuation AppSettings ErrorMailReceipients entry is missing");
            var mailSettingsSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            if (mailSettingsSection == null)
                throw new NullReferenceException("Configuation system.net/mailSettings/smtp Section is missing");
            using var email = new MailMessage{
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                Body = body
            };
            foreach (var s in errorMailRecipients.Split(';')){
                email.To.Add(s);
            }

            var title = ApplicationHelper.Instance?.Application?.Title;
            if (title!=null)
                email.ReplyToList.Add($"noreply@{title}.com");
            smtpClient.Send(email);
        }

        public static void SendMail(this Exception exception){
		    exception.ToString().SendMail($"{ApplicationHelper.Instance?.Application?.Title} Exception - {exception.GetType().FullName}");
		}

        public static DeviceCategory GetDeviceCategory(this XafApplication application) {
            if (application.GetPlatform() == Platform.Win)
                return DeviceCategory.All;
            var assemblyType = AppDomain.CurrentDomain.GetAssemblyType("DevExpress.ExpressApp.Web.DeviceDetector");
            return (DeviceCategory) Enum.Parse(typeof(DeviceCategory),$"{assemblyType.GetProperty("Instance",BindingFlags.Static|BindingFlags.Public)?.GetValue(null).CallMethod("GetDeviceCategory")}");
        }

        public static ListView CreateListView<T>(this XafApplication application, IObjectSpace objectSpace,bool isRoot=true){
            var objectType = objectSpace.TypesInfo.FindBussinessObjectType<T>();
            return application.CreateListView(objectSpace, objectType, isRoot);
        }

        public static IObjectSpace CreateObjectSpace<T>(this XafApplication application){
            return application.CreateObjectSpace(application.TypesInfo.FindBussinessObjectType<T>());
        }

        public static Platform GetPlatform(this XafApplication application){
            return application.Modules.GetPlatform();
        }

        internal static Platform GetPlatform(this IEnumerable<ModuleBase> moduleBases) {
            var modules = moduleBases as ModuleBase[] ?? moduleBases.ToArray();

            var webPlatformString = "Xaf.Platform.Web";
            var winPlatformString = "Xaf.Platform.Win";
            var mobilePlatformString = "Xaf.Platform.Mobile";
            
            if (CheckPlatform(modules, webPlatformString, winPlatformString, mobilePlatformString))
                return Platform.Web;
            if (CheckPlatform(modules, winPlatformString, webPlatformString, mobilePlatformString))
                return Platform.Win;
            if (CheckPlatform(modules, mobilePlatformString, webPlatformString, winPlatformString))
                return Platform.Mobile;
            return Platform.Agnostic;
        }

        private static bool CheckPlatform(ModuleBase[] modules, params string[] platformStrings){
            var found = CheckPlatformCore(modules, platformStrings[0]);
            if (found){
                if (!CheckPlatformCore(modules, platformStrings[1]) && !CheckPlatformCore(modules, platformStrings[2])){
                    return true;
                }
                throw new NotSupportedException("Cannot load modules from different platforms");
            }
            return false;
        }

        private static bool CheckPlatformCore(ModuleBase[] moduleBases, string platformString){
            return moduleBases.Any(@base => {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(@base.GetType());
                var attribute = typeInfo.FindAttribute<ToolboxItemFilterAttribute>();

                return attribute != null && attribute.FilterString == platformString;
            });
        }

        public static string GetStorageFolder(this XafApplication app,string folderName){
            var fileLocation = GetFileLocation(FileLocation.ApplicationFolder,folderName);
            
            switch (fileLocation){
                case FileLocation.CurrentUserApplicationDataFolder:
                    return (string) AppDomain.CurrentDomain.GetAssemblyType("System.Windows.Forms.Application").GetProperty("UserAppDataPath",BindingFlags.Public|BindingFlags.Static)?.GetValue(null);
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

        public static void SetEasyTestParameter(this XafApplication app, string parameter){
            var paramFile = Path.Combine(AppDomain.CurrentDomain.ApplicationPath() + "", "easytestparameters");
            File.AppendAllLines(paramFile,new[] {parameter});
        }

        public static bool GetEasyTestParameter(this XafApplication app,string parameter){
            if (app!=null){
                var paramFile = Path.Combine(AppDomain.CurrentDomain.ApplicationPath() + "", "easytestparameters");
                return File.Exists(paramFile) && File.ReadAllLines(paramFile).Any(s => s == parameter);
            }
            return false;
        }

        public static IEnumerable<Controller> CreateValidationControllers(this XafApplication app){
            yield return app.CreateController<ActionValidationController>();
            yield return app.CreateController<PersistenceValidationController>();
            yield return app.CreateController<ResultsHighlightController>();
        }

        public static IEnumerable<Controller> CreateAppearanceControllers(this XafApplication app){
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

        public static bool CanBuildSecurityObjects(this XafApplication xafApplication) {
            return (xafApplication.Security?.UserType != null && !xafApplication.Security.UserType.IsInterface);
        }

        public static View CreateView(this XafApplication application, IModelView viewModel) {
            return (View) application.CallMethod("CreateView", viewModel);
        }

        public static Controller CreateController(this XafApplication application,Type type){
            return (Controller) application.CallMethod(new[]{type}, "CreateController");
        }

        public static T FindModule<T>(this XafApplication xafApplication,bool exactMatch=true) where T : ModuleBase{
            var moduleType = typeof(T);
            if (moduleType.IsInterface || moduleType.IsAbstract)
                exactMatch = false;
            return !exactMatch
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

        public static int DropDatabaseOnVersionMismatch(this XafApplication xafApplication) {
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
            return xpandModule.Version != assemblyVersion;
        }

        private static void DropSqlServerDatabase(string connectionString) {
            var connectionProvider = (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None);
            using var dbConnection = connectionProvider.Connection;
            using var sqlConnection = (SqlConnection)DataStore(connectionString).Connection;
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText =
                $"ALTER DATABASE {dbConnection.Database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            sqlCommand.ExecuteNonQuery();
            sqlCommand.CommandText = $"DROP DATABASE {dbConnection.Database}";
            sqlCommand.ExecuteNonQuery();
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
            return new SimpleDataLayer(XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, cacheNode);
        }

        public static bool DisableObjectSpaceProviderCreation { get; set; }

        public static void CreateCustomObjectSpaceProvider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            if (DisableObjectSpaceProviderCreation)
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

        public static void CreateCustomObjectSpaceProvider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args, string dataStoreName) {
            if (dataStoreName == null) {
                var connectionString = ConnectionString(xafApplication, args);
                args.ObjectSpaceProvider = ObjectSpaceProvider(xafApplication,  connectionString);
            } else if (DataStoreManager.GetDataStoreAttributes(dataStoreName).Any()) {
                var disableObjectSpaceProuderCreation = DisableObjectSpaceProviderCreation;
                DisableObjectSpaceProviderCreation = false;
                CreateCustomObjectSpaceProvider(xafApplication, args);
                DisableObjectSpaceProviderCreation=disableObjectSpaceProuderCreation;
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