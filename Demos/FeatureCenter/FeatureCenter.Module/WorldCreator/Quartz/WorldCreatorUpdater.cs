using System;
using System.Configuration;
using System.IO;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.WorldCreator.Quartz {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {
        private const string Quartz = "Quartz";
        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.QueryObject<PersistentAssemblyInfo>(info => info.Name == Quartz) != null) return;

            var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), Quartz + ".xml");
            if (manifestResourceStream != null) {
                string connectionString = ConfigurationManager.ConnectionStrings["Quartz"].ConnectionString;
                var readToEnd = new StreamReader(manifestResourceStream).ReadToEnd().Replace(@"XpoProvider=MSSqlServer;data source=.\SQLEXPRESS;integrated security=SSPI;initial catalog=Quartz", connectionString);
                new ImportEngine().ImportObjects(readToEnd, info => ObjectSpace);
            }

        }
    }
}