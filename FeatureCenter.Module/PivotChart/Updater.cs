using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Xpo;

namespace FeatureCenter.Module.PivotChart
{
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion)
            : base(session, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            if (GetType() == typeof(Updater)) return;
            var name = GetName();
            if (Session.FindObject<Analysis>(analysis => analysis.Name == name) == null)
                new ImportEngine().ImportObjects(GetType().Assembly.GetManifestResourceStream(GetType(), name+".xml"), new UnitOfWork(Session.DataLayer));
        }


        protected virtual string GetName() {
            throw new NotImplementedException();
        }
    }
}
