using System;
using System.IO;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;

using Xpand.Xpo;

namespace FeatureCenter.Module.ImportExport.UpdateMembers
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            if (Session.FindObject<SerializationConfigurationGroup>(configuration => configuration.Name == "Update Members") == null) {
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "UpdateMembersGroup.xml");
                new ImportEngine().ImportObjects(stream,new UnitOfWork(Session.DataLayer));
            }
        }
    }
}
