using System;
using System.IO;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Xpo;

namespace FeatureCenter.Module.ImportExport.AnalysisObjects {
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            Import();
        }

        void Import() {
            if (Session.FindObject<Analysis>(analysis => analysis.Name == "Controlling Grid Settings") == null)
            {
                var importEngine = new ImportEngine();
                var unitOfWork = new UnitOfWork(Session.DataLayer);
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "AnalysisObjects.xml");
                importEngine.ImportObjects(stream, unitOfWork);
                stream = GetType().Assembly.GetManifestResourceStream(GetType(), "AnalysisObjectsConfiguration.xml");
                importEngine.ImportObjects(stream, unitOfWork);
            }
        }

    }
}