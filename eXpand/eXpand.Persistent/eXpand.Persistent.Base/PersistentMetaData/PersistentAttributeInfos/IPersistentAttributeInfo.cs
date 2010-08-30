namespace eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAttributeInfo : IPersistentAttributeCreator
    {
        IPersistentTypeInfo Owner { get; set; }
    }
}