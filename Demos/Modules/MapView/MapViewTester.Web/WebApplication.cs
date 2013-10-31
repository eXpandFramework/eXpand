using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Web;
using MapViewTester.Module;
using MapViewTester.Module.Web;
using MapViewTester.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using System.IO;

namespace MapViewTester.Web
{
    public class MapViewTesterAspNetApplication : WebApplication
    {
        SystemModule module1;
        SystemAspNetModule module2;
        MapViewTesterModule module3;
        MapViewTesterAspNetModule module4;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private Xpand.ExpressApp.SystemModule.XpandSystemModule xpandSystemModule1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
        private Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule xpandSystemAspNetModule1;
        private Xpand.ExpressApp.MapView.Web.MapViewWebModule _mapViewWebModule1;
        private Xpand.ExpressApp.MapView.MapViewModule mapViewModule1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;
        SqlConnection sqlConnection1;

        public MapViewTesterAspNetApplication()
        {
            InitializeComponent();
        }


        protected override void OnLoggedOn(LogonEventArgs args)
        {
            base.OnLoggedOn(args);
            using (var objectSpace = CreateObjectSpace())
            {
                if (objectSpace.GetObjectsCount(typeof(Waypoint), null) == 0)
                {
                    const string importFileName = "Waypoints.xml";
                    if (File.Exists(importFileName))
                    {
                        WaypointImporter importer = new WaypointImporter(objectSpace);
                        importer.ImportFile(@"Waypoints.xml");
                    }
                }
            }
        }
        protected override bool SupportMasterDetailMode
        {
            get { return true; }
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new XPObjectSpaceProviderThreadSafe(args.ConnectionString, args.Connection);
        }

        void MapViewTesterAspNetApplication_DatabaseVersionMismatch(object sender,
                                                                        DatabaseVersionMismatchEventArgs e)
        {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (Debugger.IsAttached)
            {
                e.Updater.Update();
                e.Handled = true;
            }
            else
            {
                string message =
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the following help topics for more detailed information:\r\n" +
                    "'Update Application and Database Versions' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm\r\n" +
                    "'Database Security References' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument3237.htm\r\n" +
                    "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/";

                if (e.CompatibilityError != null && e.CompatibilityError.Exception != null)
                {
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }

        void InitializeComponent()
        {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module3 = new MapViewTester.Module.MapViewTesterModule();
            this.module4 = new MapViewTester.Module.Web.MapViewTesterAspNetModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.xpandSystemModule1 = new Xpand.ExpressApp.SystemModule.XpandSystemModule();
            this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.xpandSystemAspNetModule1 = new Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule();
            this._mapViewWebModule1 = new Xpand.ExpressApp.MapView.Web.MapViewWebModule();
            this.mapViewModule1 = new Xpand.ExpressApp.MapView.MapViewModule();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
    "apViewTester";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securityModule1
            // 
            this.securityModule1.UserType = null;
            // 
            // validationModule1
            // 
            this.validationModule1.AllowValidationDetailsAccess = true;
            // 
            // MapViewTesterAspNetApplication
            // 
            this.ApplicationName = "MapViewTester";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.mapViewModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.xpandSystemModule1);
            this.Modules.Add(this.validationModule1);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.xpandSystemAspNetModule1);
            this.Modules.Add(this._mapViewWebModule1);
            this.Modules.Add(this.module4);
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.MapViewTesterAspNetApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}