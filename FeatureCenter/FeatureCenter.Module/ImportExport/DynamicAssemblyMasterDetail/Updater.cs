using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;

using Xpand.Xpo;

namespace FeatureCenter.Module.ImportExport.DynamicAssemblyMasterDetail {
    public class Updater:ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            if (Session.FindObject<SerializationConfigurationGroup>(@group => @group.Name == "Dynamic Assembly Master Detail")==null) {
                var importEngine = new ImportEngine();
                var unitOfWork = new UnitOfWork(Session.DataLayer);
                return;
                importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetailGroup.xml");
                importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetailModel.xml");
                importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetailModelGroup.xml");
            }
        }
    }
}