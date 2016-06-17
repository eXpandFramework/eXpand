using System.Collections.Generic;
using System.Linq;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Xpo;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.BusinessObjects{
    [NonPersistent]
    [WorldCreatorTypeInfoSource]
    public class DBLogonObject : XpandCustomObject{
        private string _connectionString;

        private string _navigationPath;

        public DBLogonObject(Session session)
            : base(session){
            _connectionString =
                "XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=Northwind";
            _navigationPath = "WorldCreator/NorthWind/BOModel/Categories";
        }

        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField]
        [RuleStringComparison("LogonObject_ConnectionString", DefaultContexts.Save, StringComparisonType.StartsWith,
            "XpoProvider")]
        public string ConnectionString{
            get { return _connectionString; }
            set { SetPropertyValue("ConnectionString", ref _connectionString, value); }
        }

        [Size(255)]
        public string NavigationPath{
            get { return _navigationPath; }
            set { SetPropertyValue("NavigationPath", ref _navigationPath, value); }
        }

        [RuleRequiredField]

        public List<DBObject> DBObjects { get; } = new List<DBObject>();


        private IEnumerable<DBObject> GetDBObjects() {
            var dataStoreSchemaExplorer =
                (IDataStoreSchemaExplorer)XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.None);
            var systemTalbes = new List<string> { "sysdiagrams", "xpobjecttype" };
            return dataStoreSchemaExplorer.GetStorageTablesList(false)
                    .Where(s => !systemTalbes.Contains(s.ToLower()))
                    .Select(s => new DBObject(Session) { Name = s });
        }

        public void PopulateTables(){
            DBObjects.AddRange(GetDBObjects());
            OnChanged("DBObjects");
        }
    }
}