using System;
using System.IO;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.ImportExport;

namespace FeatureCenter.Module.ImportExport.UpdateMembers {
    public class Updater : FCUpdater {

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }



        public override void UpdateDatabaseAfterUpdateSchema() {
            if (ObjectSpace.QueryObject<SerializationConfigurationGroup>(configuration => configuration.Name == "Update Members") == null) {
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "UpdateMembersGroup.xml");

                new ImportEngine().ImportObjects(stream, info => ObjectSpace);

            }
        }
    }
}
