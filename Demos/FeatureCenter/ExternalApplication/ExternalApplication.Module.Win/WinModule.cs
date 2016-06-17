using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;

namespace ExternalApplication.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class ExternalApplicationWindowsFormsModule : XpandModuleBase {
        public ExternalApplicationWindowsFormsModule() {
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Xpand.Persistent.BaseImpl.ImportExport.ClassInfoGraphNode)), IsExportedType));
            InitializeComponent();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return base.GetModuleUpdaters(objectSpace, versionFromDB).Where(updater => !(updater is WorldCreatorModuleUpdater));
        }

    }
}
