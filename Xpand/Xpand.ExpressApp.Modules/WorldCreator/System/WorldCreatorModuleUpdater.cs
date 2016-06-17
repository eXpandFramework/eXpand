using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Xpand.ExpressApp.WorldCreator.System {
    public abstract class WorldCreatorModuleUpdater:ModuleUpdater {
        protected WorldCreatorModuleUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){

        }
    }
}