using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;

using Xpand.Xpo;

namespace FeatureCenter.Module.ImportExport.UpdateMembers {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (session.FindObject<SerializationConfigurationGroup>(configuration => configuration.Name == "Update Members") == null) {
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "UpdateMembersGroup.xml");
                using (var unitOfWork = new UnitOfWork(session.DataLayer)) {
                    new ImportEngine().ImportObjects(stream, new ObjectSpace(unitOfWork));
                }
            }
        }
    }
}
