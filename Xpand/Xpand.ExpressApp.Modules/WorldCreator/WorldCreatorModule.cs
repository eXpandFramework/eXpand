using System;
using System.ComponentModel;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator {

    [ToolboxItem(false)]
    public sealed class WorldCreatorModule : XpandModuleBase {
        public WorldCreatorModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }

        protected override Type ApplicationType() {
            return typeof(IWorldCreatorModule);
        }


    }

}

