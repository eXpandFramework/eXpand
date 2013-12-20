using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using Xpand.Utils.Helpers;
using XVideoRental.Module.Win.BusinessObjects;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using DevExpress.ExpressApp.Utils;
using XVideoRental.Module.Win.BusinessObjects.Rent;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using System.Drawing;

namespace XVideoRental.Module.Win.DatabaseUpdate {
    public enum PermissionBehavior {
        Admin,
        Settings,
        ReadOnlyAccess
    }
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            SequenceBaseObject.Updating = true;
            var employersRole = CreateUserData();
            if (employersRole != null) {
                var importHelper = new ImportHelper(ObjectSpace,this);
                importHelper.Import();
                SetPermissions(employersRole);
            }

            CreateReports();
            CreateDashboards();

            ObjectSpace.CommitChanges();
            SequenceBaseObject.Updating = false;
        }

        void CreateDashboards() {
            CreateDashboards("Rentals", 0, new[] { typeof(Rent), typeof(MovieItem) }, ImageLoader.Instance.GetLargeImageInfo("CustomerFilmRentsList").Image);
            CreateDashboards("Customer Revenue", 1, new[] { typeof(Rent), typeof(Receipt) }, ImageLoader.Instance.GetLargeImageInfo("CustomerRevenue").Image);
            CreateDashboards("Demographics", 2, new[] { typeof(Rent) }, ImageLoader.Instance.GetLargeImageInfo("CustomersKPI").Image);
        }

        private void CreateDashboards(string dashboardName, int index, IEnumerable<Type> types, Image icon) {
            UpdateStatus("CreateDashboard", "", string.Format("Creating dashboard: {0}", dashboardName));
            var dashboard = ObjectSpace.FindObject<DashboardDefinition>(new BinaryOperator("Name", dashboardName));
            if (dashboard == null) {
                dashboard = ObjectSpace.CreateObject<DashboardDefinition>();
                dashboard.Name = dashboardName;
                dashboard.Xml = GetDashboardLayout(dashboardName).XMLPrint();
                dashboard.Icon = icon;
                dashboard.Index = index;
                dashboard.Active = true;
                foreach (var type in types)
                    dashboard.DashboardTypes.Add(new TypeWrapper(type));
            }
            var dashboardRole = ObjectSpace.GetRole("Dashboard View Role");
            var dashboardCollection = (XPBaseCollection)dashboardRole.GetMemberValue(typeof(DashboardDefinition).Name + "s");
            dashboardRole.SetMemberValue("DashboardOperation", SecurityOperationsEnum.ReadOnlyAccess);
            dashboardCollection.BaseAdd(dashboard);
        }

        private static string GetDashboardLayout(string dashboardName) {
            Type moduleType = typeof(XVideoRentalWindowsFormsModule);
            string name = moduleType.Namespace + ".Resources." + dashboardName + " Dashboard.txt";
            Stream manifestResourceStream = moduleType.Assembly.GetManifestResourceStream(name);
            if (manifestResourceStream == null)
                throw new ArgumentNullException(name);
            using (var reader = new StreamReader(manifestResourceStream))
                return reader.ReadLine();
        }

        void CreateReports() {
            CreateReport("Customer Cards", typeof(Customer));
            CreateReport("Active Customers", typeof(Customer));
            CreateReport("Most Profitable Genres", typeof(Movie));
            CreateReport("Movie Invetory", typeof(MovieItem));
            CreateReport("Movie Rentals By Customer", typeof(Customer));
            CreateReport("Top Movie Rentals", typeof(Movie));
        }

        void SetPermissions(SecuritySystemRole employersRole) {
            employersRole.SetTypePermissions<ReportData>(SecurityOperations.ReadOnlyAccess, SecuritySystemModifier.Allow);
            employersRole.CreatePermissionBehaviour(PermissionBehavior.ReadOnlyAccess, (role, info) => role.SetTypePermissions(info.Type, SecurityOperations.ReadOnlyAccess, SecuritySystemModifier.Allow));
        }

        private void CreateReport(string reportName, Type type) {
            UpdateStatus("CreateReport", "",string.Format("Creating reports: {0}", reportName));
            UpdateStatus("CreateReport","", string.Format("Creating reports: {0}", reportName));
            var reportdata = ObjectSpace.FindObject<ReportData>(new BinaryOperator("Name", reportName));
            if (reportdata == null) {
                reportdata = ObjectSpace.CreateObject<ReportData>();
                var rep = new XafReport { ObjectSpace = ObjectSpace };
                rep.LoadLayout(GetResourceStream(reportName+".repx"));
                rep.DataType = type;
                rep.ReportName = reportName;
                reportdata.SaveReport(rep);
            }
        }

        Stream GetResourceStream(string resource) {
            var moduleType = typeof(XVideoRentalWindowsFormsModule);
            return moduleType.Assembly.GetManifestResourceStream(string.Format(moduleType.Namespace + ".Resources.{0}", resource));
        }

        XpandRole CreateUserData() {
            InitAdminSecurityData();
            return InitVideoRentalSecurityData();
        }

        XpandRole InitVideoRentalSecurityData() {
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();
            if (ObjectSpace.IsNewObject(defaultRole)) {
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

        void InitAdminSecurityData() {
            var securitySystemRole = ObjectSpace.GetAdminRole("Administrator");
            securitySystemRole.GetUser("Admin");
        }


        public void CopyStream(Stream input, Stream output) {
            var buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
                output.Write(buffer, 0, bytesRead);
            }
        }

        public void SaveResource(string resource) {
            var resourceStream = GetResourceStream(resource);
            var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, resource);
            if (File.Exists(path))
                File.Delete(path);
            using (Stream output = File.Create(path)) {
                CopyStream(resourceStream, output);
            }
        }
    }

    class ImportHelper {
        readonly UnitOfWork _unitOfWork;
        readonly IObjectSpace _objectSpace;
        private readonly Updater _updater;

        public ImportHelper(IObjectSpace objectSpace, Updater updater) {
            _objectSpace = objectSpace;
            _updater = updater;
            _unitOfWork = ConnectToLegacyVideoRentDB();
        }

        UnitOfWork ConnectToLegacyVideoRentDB() {
            _updater.SaveResource("LegacyVideoRent.mdb");
            var unitOfWork = new UnitOfWork {
                ConnectionString = ConfigurationManager.ConnectionStrings["VideoRentLegacy"].ConnectionString,
                AutoCreateOption = AutoCreateOption.None
            };
            unitOfWork.Connect();
            return unitOfWork;
        }

        public UnitOfWork UnitOfWork {
            get { return _unitOfWork; }
        }

        public void Import() {
            _updater.UpdateStatus("Import", "", "");
            var initDataImporter = new InitDataImporter();
            initDataImporter.CreatingDynamicDictionary += (sender, args) => _updater.UpdateStatus("Import", "", "Creating a dynamic dictionary...");
            initDataImporter.TransformingRecords += (sender, args) => NotifyWhenTransform(args.InputClassName, args.Position);
            initDataImporter.CommitingData += (sender, args) => _updater.UpdateStatus("Import", "", "Commiting data...");

            initDataImporter.Import(() => new UnitOfWork(((XPObjectSpace)_objectSpace).Session.ObjectLayer), () => new UnitOfWork(_unitOfWork.ObjectLayer));

        }
        void NotifyWhenTransform(string inputClassName, int position) {
            var statusMessage = position > -1
                                       ? string.Format("Transforming records from {0}: {1}", inputClassName, position)
                                       : string.Format("Transforming records from {0} ...", inputClassName);
            _updater.UpdateStatus("Import", "", statusMessage);
        }

    }
}