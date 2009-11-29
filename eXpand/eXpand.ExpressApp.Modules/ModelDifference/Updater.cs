using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ModelDifference
{
    public class Updater : ModuleUpdater
    {
        [CoverageExclude]
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion){
            
        }

    }
}