using System;
using System.Collections.Generic;
using System.Web;
using CThru;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.WorldCreator.CThru
{

    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
        }
    }
}


