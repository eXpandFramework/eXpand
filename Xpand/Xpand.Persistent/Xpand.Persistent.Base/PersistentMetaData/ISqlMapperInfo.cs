namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface ISqlMapperInfo : IDataStoreLogonObject
    {
        IMapperInfo MapperInfo { get; set; }
    }
}