using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;

using Xpand.Xpo;

namespace FeatureCenter.Module.ImportExport.AnalysisObjects {
    public class Updater : FCUpdater {

        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion, updater) {
        }


        public override void UpdateDatabaseAfterUpdateSchema() {
            Import();
        }

        void Import() {
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (session.FindObject<Analysis>(analysis => analysis.Name == "Controlling Grid Settings") == null) {
                var importEngine = new ImportEngine();
                using (var unitOfWork = new UnitOfWork(session.DataLayer)) {
                    Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "AnalysisObjects.xml");
                    importEngine.ImportObjects(stream, new ObjectSpace(unitOfWork));
                    stream = GetType().Assembly.GetManifestResourceStream(GetType(), "AnalysisObjectsConfiguration.xml");
                    importEngine.ImportObjects(stream, new ObjectSpace(unitOfWork));
                }
            }
        }
    }
}