using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Xpand.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;

using Xpand.Xpo;

namespace FeatureCenter.Module.ImportExport.UpdateMembers {
    public class Updater : FCUpdater {

        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion, updater) {
        }



        public override void UpdateDatabaseAfterUpdateSchema() {
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (session.FindObject<SerializationConfigurationGroup>(configuration => configuration.Name == "Update Members") == null) {
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "UpdateMembersGroup.xml");

                new ImportEngine().ImportObjects(stream, new XPObjectSpace(XafTypesInfo.Instance, XpandModuleBase.XpoTypeInfoSource, () => new UnitOfWork(session.DataLayer)));

            }
        }
    }
}
