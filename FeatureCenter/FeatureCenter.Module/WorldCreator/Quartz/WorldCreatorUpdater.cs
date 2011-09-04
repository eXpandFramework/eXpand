using System.Configuration;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.Quartz {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater {
        private const string Quartz = "Quartz";
        public WorldCreatorUpdater(Session session)
            : base(session) {
        }
        public override void Update() {
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name == Quartz) != null) return;
            using (var unitOfWork = new UnitOfWork(Session.DataLayer)) {
                var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), Quartz + ".xml");
                if (manifestResourceStream != null) {
                    string connectionString = ConfigurationManager.ConnectionStrings["Quartz"].ConnectionString;
                    var readToEnd = new StreamReader(manifestResourceStream).ReadToEnd().Replace(@"XpoProvider=MSSqlServer;data source=.\SQLEXPRESS;integrated security=SSPI;initial catalog=Quartz", connectionString);
                    new ImportEngine().ImportObjects(readToEnd, new ObjectSpace(unitOfWork));
                }

            }
        }
    }
}