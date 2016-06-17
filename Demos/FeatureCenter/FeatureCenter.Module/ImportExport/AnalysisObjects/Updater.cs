using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.ImportExport.AnalysisObjects {
    public class Updater : FCUpdater {

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }


        public override void UpdateDatabaseAfterUpdateSchema() {
            Import();
        }

        void Import() {
            if (ObjectSpace.QueryObject<Analysis>(analysis => analysis.Name == "Controlling Grid Settings") == null) {
                var importEngine = new ImportEngine();
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "AnalysisObjects.xml");
                importEngine.ImportObjects(stream, info => ObjectSpace);
                stream = GetType().Assembly.GetManifestResourceStream(GetType(), "AnalysisObjectsConfiguration.xml");
                importEngine.ImportObjects(stream, info => ObjectSpace);
            }
        }
    }
}