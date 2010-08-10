using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Xpo;

namespace FeatureCenter.Module.IO.AnalysisObjects {
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            Import("Controlling Grid Settings");
            Import("PivotGroupInterval");
            Import("Custom Sort");
            Import("HideChart");
            Import("HidePivot");
            Import("InPlaceEdit");
            
        }

        void Import(string name) {
            if (Session.FindObject<Analysis>(analysis => analysis.Name == name) == null)
                new ImportEngine().ImportObjects(GetType().Assembly.GetManifestResourceStream(GetType(), name + ".xml"), new UnitOfWork(Session.DataLayer));
        }

    }
}