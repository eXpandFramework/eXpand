using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using XVideoRental.Module.Win.BusinessObjects;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using DevExpress.ExpressApp.Utils;
using XVideoRental.Module.Win.BusinessObjects.Rent;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using Xpand.Utils.Automation;
using System.Drawing;

namespace XVideoRental.Module.Win.DatabaseUpdate
{
    public enum PermissionBehavior
    {
        Admin,
        Settings,
        ReadOnlyAccess
    }
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            SequenceBaseObject.Updating = true;
            var employersRole = CreateUserData();
            if (employersRole != null)
            {
                var importHelper = new ImportHelper(ObjectSpace);
                importHelper.Import();
                SetPermissions(employersRole);
            }

            CreateReports();
            CreateDashboards();

            ObjectSpace.CommitChanges();
            SequenceBaseObject.Updating = false;
        }

        void CreateDashboards()
        {
            CreateDashboards("Rentals", 0, new Type[] { typeof(Rent), typeof(MovieItem) }, ImageLoader.Instance.GetLargeImageInfo("CustomerFilmRentsList").Image);
            CreateDashboards("Customer Revenue", 1, new Type[] { typeof(Rent), typeof(Receipt) }, ImageLoader.Instance.GetLargeImageInfo("CustomerRevenue").Image);
            CreateDashboards("Demographics", 2, new Type[] { typeof(Rent) }, ImageLoader.Instance.GetLargeImageInfo("CustomersKPI").Image);
        }

        private void CreateDashboards(string dashboardName, int index, Type[] types, Image icon)
        {
            ApplicationStatusUpdater.Notify("CreateDashboard", string.Format("Creating dashboard: {0}", dashboardName));
            var dashboard = ObjectSpace.FindObject<DashboardDefinition>(new BinaryOperator("Name", dashboardName));
            if (dashboard == null)
            {
                dashboard = ObjectSpace.CreateObject<DashboardDefinition>();
                dashboard.Name = dashboardName;
                dashboard.Xml = GetDashboardLayout(dashboardName);
                dashboard.Icon = icon;
                dashboard.Index = index;
                dashboard.Active = true;
                foreach (var type in types)
                    dashboard.DashboardTypes.Add(new TypeWrapper(((XPObjectSpace)ObjectSpace).Session, type));
            }
            var dashboardRole = ObjectSpace.GetRole("Dashboard View Role");
            var dashboardCollection = (XPBaseCollection)dashboardRole.GetMemberValue(typeof(DashboardDefinition).Name + "s");
            dashboardCollection.BaseAdd(dashboard);
        }

        private static string GetDashboardLayout(string dashboardName)
        {
            Type moduleType = typeof(XVideoRentalWindowsFormsModule);
            string name = moduleType.Namespace + ".Resources." + dashboardName + " Dashboard.txt";
            Stream manifestResourceStream = moduleType.Assembly.GetManifestResourceStream(name);
            if (manifestResourceStream == null)
                throw new ArgumentNullException(name);
            using (var reader = new StreamReader(manifestResourceStream))
                return reader.ReadLine();
        }

        void CreateReports()
        {
            CreateReport("Customer Cards", typeof(Customer));
            CreateReport("Active Customers", typeof(Customer));
            CreateReport("Most Profitable Genres", typeof(Movie));
            CreateReport("Movie Invetory", typeof(MovieItem));
            CreateReport("Movie Rentals By Customer", typeof(Customer));
            CreateReport("Top Movie Rentals", typeof(Movie));
        }

        void SetPermissions(SecuritySystemRole employersRole)
        {
            employersRole.SetTypePermissions<ReportData>(SecurityOperations.ReadOnlyAccess, SecuritySystemModifier.Allow);
            employersRole.CreatePermissionBehaviour(PermissionBehavior.ReadOnlyAccess, (role, info) => role.SetTypePermissions(info.Type, SecurityOperations.ReadOnlyAccess, SecuritySystemModifier.Allow));
        }

        private void CreateReport(string reportName, Type type)
        {
            ApplicationStatusUpdater.Notify("CreateReport", string.Format("Creating reports: {0}", reportName));
            var reportdata = ObjectSpace.FindObject<ReportData>(new BinaryOperator("Name", reportName));
            if (reportdata == null)
            {
                reportdata = ObjectSpace.CreateObject<ReportData>();
                var rep = new XafReport { ObjectSpace = ObjectSpace };
                rep.LoadLayout(GetReportStream(reportName));
                rep.DataType = type;
                rep.ReportName = reportName;
                reportdata.SaveReport(rep);
            }
        }

        Stream GetReportStream(string reportName)
        {
            var moduleType = typeof(XVideoRentalWindowsFormsModule);
            return moduleType.Assembly.GetManifestResourceStream(string.Format(moduleType.Namespace + ".Resources.{0}.repx", reportName));
        }

        XpandRole CreateUserData()
        {
            InitAdminSecurityData();
            return InitVideoRentalSecurityData();
        }

        XpandRole InitVideoRentalSecurityData()
        {
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();
            if (ObjectSpace.IsNewObject(defaultRole))
            {
                var employersRole = (SecuritySystemRole)ObjectSpace.GetRole("Employers");
                var dashboardRole = (SecuritySystemRole)ObjectSpace.GetRole("Dashboard View Role");

                var user = (SecuritySystemUser)employersRole.GetUser("User");
                var dashboardUser = (SecuritySystemUser)dashboardRole.GetUser("DashboardUser");

                user.Roles.Add(defaultRole);
                dashboardUser.Roles.Add(defaultRole);
                dashboardUser.Roles.Add(dashboardRole);

                employersRole.CreateFullPermissionAttributes();
                return (XpandRole)employersRole;
            }
            return null;
        }

        void InitAdminSecurityData()
        {
            var securitySystemRole = ObjectSpace.GetAdminRole("Administrator");
            securitySystemRole.GetUser("Admin");
        }
    }

    class ImportHelper
    {
        readonly UnitOfWork _unitOfWork;
        readonly IObjectSpace _objectSpace;

        public ImportHelper(IObjectSpace objectSpace)
        {
            _objectSpace = objectSpace;
            _unitOfWork = ConnectToLegacyVideoRentDB();
            CreateViews();
        }
        UnitOfWork ConnectToLegacyVideoRentDB()
        {
            var unitOfWork = new UnitOfWork
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["VideoRentLegacy"].ConnectionString,
                AutoCreateOption = AutoCreateOption.None
            };
            try
            {
                unitOfWork.Connect();
            }
            catch (UnableToOpenDatabaseException)
            {
                if (StartVideoRent(unitOfWork))
                    unitOfWork.Connect();
                else
                    Application.ExitThread();
            }
            return unitOfWork;
        }

        bool StartVideoRent(UnitOfWork unitOfWork)
        {
            string videoRentalPath = Environment.ExpandEnvironmentVariables(string.Format(ConfigurationManager.AppSettings["VideoRentLegacyPath"], AssemblyInfo.VersionShort));
            var dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("XVideoRental uses the initial data from the database created by the original VideoRental application that is installed with our WinForms components at \r\n{0}.\r\n\r\nChoose 'Yes' to automatically run the WinForms VideRental application and create the required SQL Express database by default (application restart is required).\r\nChoose 'No' to do this manually later and exit this application for now.", videoRentalPath),
                    "Initial data was not found...", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                if (!File.Exists(videoRentalPath))
                {
                    throw new FileNotFoundException(string.Format("Cannot run the WinForms VideoRental application by the following path: {0}", videoRentalPath));
                }
                DeleteMdbFiles();
                Process videoRental = Process.Start(videoRentalPath);
                if (videoRental != null)
                {
                    WaitAutomation.WaitForWindowToOpen("Create Database");
                    WaitAutomation.WaitForWindowToClose("Create Database");
                    if (!LegacyDbExists(unitOfWork))
                        return false;
                    WaitAutomation.WaitForWindowToOpen("About - Video Rental Demo (C#)");
                    videoRental.Kill();
                    return true;
                }
                throw new ApplicationException("The legacy VideoRent application failed to start");
            }
            return false;
        }

        void DeleteMdbFiles()
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath) + "", "*.mdb");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        bool LegacyDbExists(UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Connect();
                return true;
            }
            catch (UnableToOpenDatabaseException)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Application will now exit because you have not created the legacy database!!!", "Exit", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.ExitThread();
            }
            return false;
        }


        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        void CreateViews()
        {
            if (_unitOfWork != null && _unitOfWork.Connection != null)
            {
                var dbCommand = _unitOfWork.Connection.CreateCommand();
                ApplicationStatusUpdater.Notify("CreateSqlViews", "Creating SQL views...");
                CreatePersonView(dbCommand, "Artist", "vArtist");
                CreatePersonView(dbCommand, "Customer", "vCustomer");
                CreatePictureView(dbCommand, "ArtistPicture", "vArtistPicture");
                CreatePictureView(dbCommand, "MoviePicture", "vMoviePicture");
            }
        }

        void CreatePictureView(IDbCommand dbCommand, string tableName, string viewName)
        {
            DropView(dbCommand, viewName);
            dbCommand.CommandText = string.Format("CREATE VIEW [dbo].[{0}] AS " +
                    "SELECT dbo.Picture.Image, dbo.Picture.Description, dbo.{1}.* " +
                    "FROM dbo.{1} INNER JOIN dbo.Picture ON dbo.{1}.Oid = dbo.Picture.Oid",
                    viewName, tableName);
            dbCommand.ExecuteNonQuery();
        }

        void CreatePersonView(IDbCommand dbCommand, string tableName, string viewName)
        {
            DropView(dbCommand, viewName);
            dbCommand.CommandText = string.Format("CREATE VIEW [dbo].[{0}] AS " +
                    "SELECT dbo.Person.FirstName, dbo.Person.LastName, dbo.Person.Gender, dbo.Person.BirthDate, dbo.{1}.* " +
                    "FROM dbo.{1} INNER JOIN dbo.Person ON dbo.{1}.Oid = dbo.Person.Oid",
                    viewName, tableName);
            dbCommand.ExecuteNonQuery();
        }

        void DropView(IDbCommand dbCommand, string viewName)
        {
            try
            {
                dbCommand.CommandText = string.Format("DROP VIEW [dbo].[{0}]", viewName);
                dbCommand.ExecuteNonQuery();
            }
            catch (SqlException)
            {
            }
        }

        public void Import()
        {
            ApplicationStatusUpdater.Notify("Import", "");
            DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show("This operation may take a few minutes, please wait. Press OK to continue.", "Importing and transforming  initial data...", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel)
            {
                Environment.Exit(Environment.ExitCode);
            }
            var initDataImporter = new InitDataImporter();
            initDataImporter.CreatingDynamicDictionary += (sender, args) => ApplicationStatusUpdater.Notify("Import", "Creating a dynamic dictionary...");
            initDataImporter.TransformingRecords += (sender, args) => NotifyWhenTransform(args.InputClassName, args.Position);
            initDataImporter.CommitingData += (sender, args) => ApplicationStatusUpdater.Notify("Import", "Commiting data...");

            initDataImporter.Import(() => new UnitOfWork(((XPObjectSpace)_objectSpace).Session.ObjectLayer), () => new UnitOfWork(_unitOfWork.ObjectLayer));

        }
        void NotifyWhenTransform(string inputClassName, int position)
        {
            var statusMessage = position > -1
                                       ? string.Format("Transforming records from {0}: {1}", inputClassName, position)
                                       : string.Format("Transforming records from {0} ...", inputClassName);
            ApplicationStatusUpdater.Notify("Import", statusMessage);
        }
    }
}