using System;
using System.Configuration;
using System.IO;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.WorldCreator.NorthWind {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {
        private const string NorthWind = "NorthWind";
        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.QueryObject<PersistentAssemblyInfo>(info => info.Name == NorthWind) != null) return;

            var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), NorthWind + ".xml");
            if (manifestResourceStream != null) {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings["NorthWind"];
                if (connectionStringSettings != null) {
                    string connectionString = connectionStringSettings.ConnectionString;
                    var readToEnd = new StreamReader(manifestResourceStream).ReadToEnd().Replace(@"XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=Northwind", connectionString);
                    new ImportEngine().ImportObjects(readToEnd, info => ObjectSpace);
                }
            }

        }
    }
}