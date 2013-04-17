using System;
using System.Diagnostics;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.WorldCreator.Win {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            KeepOnlyCurrentVersionDXAssemblies();
        }

        void KeepOnlyCurrentVersionDXAssemblies() {
            var applicationFolder = PathHelper.GetApplicationFolder();
            var files = Directory.GetFiles(applicationFolder, "DevExpress*.dll");
            foreach (var file in files) {
                if (!FileVersionInfo.GetVersionInfo(file).FileVersion.StartsWith(AssemblyInfo.VersionShort))
                    File.Delete(file);
            }

        }
    }
}