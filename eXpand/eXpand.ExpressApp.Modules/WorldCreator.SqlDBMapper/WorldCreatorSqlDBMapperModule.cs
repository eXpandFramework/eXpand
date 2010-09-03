namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper
{
    public sealed class WorldCreatorSqlDBMapperModule : XpandModuleBase
    {
        public WorldCreatorSqlDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

