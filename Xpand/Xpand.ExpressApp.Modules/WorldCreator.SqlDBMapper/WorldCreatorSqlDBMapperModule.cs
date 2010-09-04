using Xpand.ExpressApp;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper
{
    public sealed class WorldCreatorSqlDBMapperModule : XpandModuleBase
    {
        public WorldCreatorSqlDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

