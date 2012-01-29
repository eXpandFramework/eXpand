using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    [ToolboxBitmap(typeof(WorldCreatorDBMapperModule))]
    [ToolboxItem(true)]
    public sealed class WorldCreatorDBMapperModule : XpandModuleBase {
        public WorldCreatorDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

