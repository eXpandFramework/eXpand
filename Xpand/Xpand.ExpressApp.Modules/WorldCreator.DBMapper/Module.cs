using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Xpand.ExpressApp.WorldCreator.DBMapper.BusinessObjects;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    [ToolboxBitmap(typeof(WorldCreatorDBMapperModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class WorldCreatorDBMapperModule : XpandModuleBase {
        public WorldCreatorDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (RuntimeMode)
                WorldCreatorTypeInfoSource.Instance.ForceRegisterEntity(typeof(DBLogonObject));
        }
    }
}

