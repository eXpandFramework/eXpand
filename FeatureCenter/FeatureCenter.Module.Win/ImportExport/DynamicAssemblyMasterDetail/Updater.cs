using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Xpo;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.Session.FindObject<SerializationConfigurationGroup>(@group => @group.Name == "Dynamic Assembly Master Detail") == null) {
                var importEngine = new ImportEngine();
                using (var unitOfWork = new UnitOfWork(ObjectSpace.Session.DataLayer)) {
                    importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetailGroup.xml");
                    importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetailModel.xml");
                    importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetailModelGroup.xml");
                }
            }
        }
    }
}