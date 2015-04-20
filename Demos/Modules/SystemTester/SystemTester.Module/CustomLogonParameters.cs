using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace SystemTester.Module{
    public enum DbServerType{
        Default,
        SqlLite,
        MySql
    }
    [NonPersistent]
    public class CustomLogonParameters : AuthenticationStandardLogonParameters {
        public DbServerType DbServerType { get; set; }
    }
}