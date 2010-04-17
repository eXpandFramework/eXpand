using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.WorldCreator.CThru
{
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
    }
}


