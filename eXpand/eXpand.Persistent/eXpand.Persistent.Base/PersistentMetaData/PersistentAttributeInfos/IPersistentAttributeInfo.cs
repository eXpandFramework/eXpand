namespace eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAttributeInfo {
        IPersistentTypeInfo Owner { get; set; }
        AttributeInfo Create();
    }
}