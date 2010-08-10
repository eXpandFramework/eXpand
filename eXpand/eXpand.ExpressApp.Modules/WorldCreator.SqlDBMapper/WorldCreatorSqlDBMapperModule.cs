namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper
{
    public sealed class WorldCreatorSqlDBMapperModule : ModuleBase
    {
        public WorldCreatorSqlDBMapperModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
        }
    }

}

