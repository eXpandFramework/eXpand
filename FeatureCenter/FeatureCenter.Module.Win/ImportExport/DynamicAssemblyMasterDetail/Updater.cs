using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Xpo;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (session.FindObject<SerializationConfigurationGroup>(@group => @group.Name == "Dynamic Assembly Master Detail") == null) {
                var importEngine = new ImportEngine();
                using (var unitOfWork = new UnitOfWork(session.DataLayer)) {
                    importEngine.ImportObjects(new ObjectSpace(unitOfWork), GetType(), "DynamicAssemblyMasterDetailGroup.xml");
                    importEngine.ImportObjects(new ObjectSpace(unitOfWork), GetType(), "DynamicAssemblyMasterDetailModel.xml");
                    importEngine.ImportObjects(new ObjectSpace(unitOfWork), GetType(), "DynamicAssemblyMasterDetailModelGroup.xml");
                }
            }
        }
    }
}