using System.Configuration;
using System.IO;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;

namespace IOTester.Module.FunctionalTests.NorthWind {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater {
        private const string NorthWind = "NorthWind";
        public WorldCreatorUpdater(Session session)
            : base(session) {
        }
        public override void Update() {
            if (Session.FindObject<SecuritySystemRole>(null) == null) {
                var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), NorthWind + ".xml");
                if (manifestResourceStream != null) {
                    var connectionStringSettings = ConfigurationManager.ConnectionStrings["NorthWind"];
                    if (connectionStringSettings != null) {
                        string connectionString = connectionStringSettings.ConnectionString;
                        var readToEnd = new StreamReader(manifestResourceStream).ReadToEnd().Replace(@"XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=Northwind", connectionString);
                        new ImportEngine().ImportObjects(readToEnd, new UnitOfWork(Session.DataLayer));
                    }
                }
            }

        }
    }
}