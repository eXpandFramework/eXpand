using System.ComponentModel;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Validation;

namespace Xpand.ExpressApp.WorldCreator {

    [ToolboxItem(false)]
    public sealed class WorldCreatorModule : XpandModuleBase {
        public WorldCreatorModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (!XpandDesignMode)
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.PersistentMetaData");
        }

    }

}

