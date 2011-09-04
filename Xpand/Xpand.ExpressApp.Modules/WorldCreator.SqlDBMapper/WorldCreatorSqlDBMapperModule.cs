using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper {
    [ToolboxBitmap(typeof(WorldCreatorSqlDBMapperModule))]
    [ToolboxItem(true)]
    public sealed class WorldCreatorSqlDBMapperModule : XpandModuleBase {
        public WorldCreatorSqlDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

