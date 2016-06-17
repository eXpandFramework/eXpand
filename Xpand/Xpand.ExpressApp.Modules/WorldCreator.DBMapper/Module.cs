using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Utils;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    [ToolboxBitmap(typeof(WorldCreatorDBMapperModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class WorldCreatorDBMapperModule : XpandModuleBase {
        public WorldCreatorDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);
            var types = ModuleHelper.CollectExportedTypesFromAssembly(GetType().Assembly,IsExportedType);
            types.Each(WorldCreatorTypeInfoSource.Instance.ForceRegisterEntity);
        }
    }
}

