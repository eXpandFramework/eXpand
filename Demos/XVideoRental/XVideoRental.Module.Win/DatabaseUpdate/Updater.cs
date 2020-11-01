using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Helpers;
using XVideoRental.Module.Win.BusinessObjects;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.BusinessObjects.Rent;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.Security.Core;
using System.Drawing;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Xpand.Extensions.XAF.SecurityExtensions;
using Country = XVideoRental.Module.Win.BusinessObjects.Movie.Country;

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

            CreateDashboards();

            ObjectSpace.CommitChanges();
            SequenceBaseObject.Updating = false;
        }

        void CreateDashboards() {
            CreateDashboards("Rentals", 0, new[] { typeof(Rent), typeof(MovieItem) });
            CreateDashboards("Customer Revenue", 1, new[] { typeof(Rent), typeof(Receipt) });
            CreateDashboards("Demographics", 2, new[] { typeof(Rent) });
        }

        private void CreateDashboards(string dashboardName, int index, IEnumerable<Type> types) {
            UpdateStatus("CreateDashboard", "", $"Creating dashboard: {dashboardName}");
            var dashboard = ObjectSpace.FindObject<DashboardDefinition>(new BinaryOperator("Name", dashboardName));
            if (dashboard == null) {
                dashboard = ObjectSpace.CreateObject<DashboardDefinition>();
                dashboard.Name = dashboardName;
                dashboard.Xml = GetDashboardLayout(dashboardName).XMLPrint();
                dashboard.Index = index;
                dashboard.Active = true;
                foreach (var type in types)
                    dashboard.DashboardTypes.Add(new TypeWrapper(type));
            }
            var dashboardRole = ObjectSpace.GetRole("Dashboard View Role");
            var dashboardCollection = (XPBaseCollection)
                ((XPBaseObject) dashboardRole).GetMemberValue(typeof(DashboardDefinition).Name + "s");
            ((XPBaseObject) dashboardRole).SetMemberValue("DashboardOperation", SecurityOperationsEnum.ReadOnlyAccess);
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

        void SetPermissions(IPermissionPolicyRole employersRole) {
            employersRole.SetTypePermission<ReportData>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            ((ISecurityRole) employersRole).CreatePermissionBehaviour(PermissionBehavior.ReadOnlyAccess, (role, info) =>
                ((IPermissionPolicyRole) role).SetTypePermission(info.Type, SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow));
        }

        Stream GetResourceStream(string resource) {
            var moduleType = typeof(XVideoRentalWindowsFormsModule);
            return moduleType.Assembly.GetManifestResourceStream(string.Format(moduleType.Namespace + ".Resources.{0}", resource));
        }

        IPermissionPolicyRole CreateUserData() {
            InitAdminSecurityData();
            return InitVideoRentalSecurityData();
        }

        IPermissionPolicyRole InitVideoRentalSecurityData() {
            var defaultRole = (PermissionPolicyRole)ObjectSpace.GetDefaultRole();
            if (ObjectSpace.IsNewObject(defaultRole)) {
                var employersRole = (IPermissionPolicyRole)ObjectSpace.GetRole("Employers");
                var dashboardRole = (PermissionPolicyRole)ObjectSpace.GetRole("Dashboard View Role");

                var user = (PermissionPolicyUser) ((ISecurityRole) employersRole).GetUser("User");
                var dashboardUser = (PermissionPolicyUser) dashboardRole.GetUser("DashboardUser");

                user.Roles.Add(defaultRole);
                dashboardUser.Roles.Add(defaultRole);
                dashboardUser.Roles.Add(dashboardRole);

                ((ISecurityRole) employersRole).AddNewFullPermissionAttributes();
                return employersRole;
            }
            return null;
        }

        void InitAdminSecurityData() {
            var securitySystemRole = ObjectSpace.GetAdminRole("Administrator");
            ObjectSpace.GetUser("Admin",roles:(ISecurityRole) securitySystemRole);
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

        void Decompress(FileInfo fileInfo) {
            using (FileStream inFile = fileInfo.OpenRead()) {
                string curFile = fileInfo.FullName;
                string origName = curFile.Remove(curFile.Length - fileInfo.Extension.Length) + ".mdb";
                using (FileStream outFile = File.Create(origName)) {
                    using (var decompress = new GZipStream(inFile, CompressionMode.Decompress)) {
                        decompress.CopyTo(outFile);
                    }
                }
            }
        }

        UnitOfWork ConnectToLegacyVideoRentDB() {
            _updater.SaveResource("LegacyVideoRent.zip");
            Decompress(new FileInfo("LegacyVideoRent.zip"));
            var unitOfWork = new UnitOfWork {
                ConnectionString = ConfigurationManager.ConnectionStrings["VideoRentLegacy"].ConnectionString,
                AutoCreateOption = AutoCreateOption.None
            };
            unitOfWork.Connect();
            return unitOfWork;
        }

        public UnitOfWork UnitOfWork => _unitOfWork;

        public void Import() {
            _updater.UpdateStatus("Import", "", "");
            var initDataImporter = new InitDataImporter();
            initDataImporter.CreatingDynamicDictionary += (sender, args) => _updater.UpdateStatus("Import", "", "Creating a dynamic dictionary...");
            initDataImporter.TransformingRecords += (sender, args) => NotifyWhenTransform(args.InputClassName, args.Position);
            initDataImporter.CommitingData += (sender, args) => _updater.UpdateStatus("Import", "", "Commiting data...");

            initDataImporter.Import(() => new UnitOfWork(((XPObjectSpace)_objectSpace).Session.ObjectLayer), () => new UnitOfWork(_unitOfWork.ObjectLayer));
            UpdatePhotosFromReusableStorage();
        }

        private UnitOfWork GetXmlUnitOfWork() {
            var reflectionDictionary = new ReflectionDictionary();
            var appSetting = string.Format(ConfigurationManager.AppSettings["VideoRentLegacyPath"], AssemblyInfo.VersionShort);
            var fullPath = Environment.ExpandEnvironmentVariables(appSetting);

            var legacyAssembly = Assembly.LoadFrom(Path.Combine(fullPath + @"\bin", @"DevExpress.VideoRent.dll"));
            reflectionDictionary.CollectClassInfos(legacyAssembly);
            var inMemoryDataStore = new InMemoryDataStore();
            inMemoryDataStore.ReadXml(Path.Combine(fullPath + @"\CS\DevExpress.VideoRent\Data", @"VideoRent.xml"));
            var simpleDataLayer = new SimpleDataLayer(reflectionDictionary, inMemoryDataStore);
            return new UnitOfWork(simpleDataLayer);
        }

        private void UpdatePhotosFromReusableStorage() {
            var xmlUnitOfWork = GetXmlUnitOfWork();
            _objectSpace.CommitChanges();
            foreach (var type in new[] { typeof(Customer), typeof(Movie), typeof(Country), typeof(MoviePicture), typeof(ArtistPicture) }) {
                string memberName = type.Name + "Id";
                Func<VideoRentalBaseObject, object> parameters = o => o.GetMemberValue("Id");
                if (type == typeof(Country)) {
                    memberName = "Name";
                    parameters = o => o.GetMemberValue(memberName);
                }
                else if (type == typeof(MoviePicture)) {
                    memberName = "Movie.MovieId";
                    parameters = o => ((XPBaseObject)o.GetMemberValue("Movie")).GetMemberValue("Id");
                }
                else if (type == typeof(ArtistPicture)) {
                    memberName = "Artist.ArtistId";
                    parameters = o => ((XPBaseObject)o.GetMemberValue("Artist")).GetMemberValue("Id");
                }
                UpdateImage(type, xmlUnitOfWork, memberName, parameters);
            }
            _objectSpace.CommitChanges();
        }

        private void UpdateImage(Type type, UnitOfWork xmlUnitOfWork, string memberName, Func<VideoRentalBaseObject, object> parameters) {
            foreach (var rentalBaseObject in _objectSpace.GetObjects(type, null, true).Cast<VideoRentalBaseObject>()) {
                var classInfo = xmlUnitOfWork.Dictionary.Classes.OfType<XPClassInfo>().First(info => info.TableName == type.Name);
                var baseObject = (XPBaseObject)xmlUnitOfWork.FindObject(classInfo, CriteriaOperator.Parse(memberName + "=?", parameters(rentalBaseObject)));
                var memberInfos = rentalBaseObject.ClassInfo.Members.Where(info => info.MemberType == typeof(Image) && info.IsPublic);
                foreach (var memberInfo in memberInfos) {
                    _updater.UpdateStatus("Import", "Import", "Updating " + type.Name + " " + memberInfo.Name + "s");
                    memberInfo.SetValue(rentalBaseObject, baseObject.GetMemberValue(memberInfo.Name));
                }
            }
        }


        void NotifyWhenTransform(string inputClassName, int position) {
            var statusMessage = position > -1
                                       ? $"Transforming records from {inputClassName}: {position}"
                : $"Transforming records from {inputClassName} ...";
            _updater.UpdateStatus("Import", "", statusMessage);
        }

    }
}