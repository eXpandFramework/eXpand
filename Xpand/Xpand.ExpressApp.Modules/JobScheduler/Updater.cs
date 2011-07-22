using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }
    }
}
