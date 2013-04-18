using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    [ToolboxBitmap(typeof(WorldCreatorDBMapperModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    public sealed class WorldCreatorDBMapperModule : XpandModuleBase {
        public WorldCreatorDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

