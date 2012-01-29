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
            ConnectionString ="XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=Northwind";
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
        [Size(255)]
        public string NavigationPath {
            get {
                return _navigationPath;
            }
            set {
                SetPropertyValue("NavigationPath", ref _navigationPath, value);
            }
        }
    }
}
