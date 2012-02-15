using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {

    [NonPersistent]
    public class LogonObject : XpandCustomObject {
        public LogonObject(Session session)
            : base(session) {
            ConnectionString = "XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=Northwind";
            NavigationPath = "WorldCreator/NorthWind/BOModel/Categories";
        }
        private string _connectionString;
        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField]
        [RuleStringComparison("LogonObject_ConnectionString", DefaultContexts.Save, StringComparisonType.StartsWith, "XpoProvider")]
        public string ConnectionString {
            get {
                return _connectionString;
            }
            set {
                SetPropertyValue("ConnectionString", ref _connectionString, value);
            }
        }
        protected override void OnSaving() {
            base.OnSaving();
            if (!(ConnectionString + "").ToLower().StartsWith(DataStoreBase.XpoProviderTypeParameterName.ToLower()))
                throw new UserFriendlyException("Connectionstring should start with " + DataStoreBase.XpoProviderTypeParameterName);
        }
        private string _navigationPath;

        XPCollection<DataTable> _dataTables;

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "ConnectionString") {
                _dataTables = new XPCollection<DataTable>(Session, GetStorageTables());
                OnChanged("DataTables");
            }
        }

        IEnumerable<DataTable> GetStorageTables() {
            var dataStoreSchemaExplorer = (IDataStoreSchemaExplorer)XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.None);
            var systemTalbes = new List<string> { "sysdiagrams", "xpobjecttype" };
            return dataStoreSchemaExplorer.GetStorageTablesList().Where(s => !systemTalbes.Contains(s.ToLower())).Select(s => new DataTable(Session) { Name = s });
        }

        [Size(255)]
        public string NavigationPath {
            get {
                return _navigationPath;
            }
            set {
                SetPropertyValue("NavigationPath", ref _navigationPath, value);
            }
        }

        public XPCollection<DataTable> DataTables {
            get {
                
                return _dataTables;
            }
        }
    }

    public class DataTable : XpandCustomObject {
        public DataTable(Session session)
            : base(session) {
        }
        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        
    }
}
