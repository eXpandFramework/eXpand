using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    [ToolboxBitmap(typeof(WorldCreatorDBMapperModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class WorldCreatorDBMapperModule : XpandModuleBase {
        public WorldCreatorDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

