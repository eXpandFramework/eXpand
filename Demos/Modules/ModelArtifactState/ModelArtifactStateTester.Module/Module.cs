using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;


namespace ModelArtifactStateTester.Module {
    public sealed partial class ModelArtifactStateTesterModule : ModuleBase {
        public ModelArtifactStateTesterModule() {
            InitializeComponent();
            RequiredModuleTypes.Add(typeof(Xpand.XAF.Modules.CloneModelView.CloneModelViewModule));
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
    }
}
