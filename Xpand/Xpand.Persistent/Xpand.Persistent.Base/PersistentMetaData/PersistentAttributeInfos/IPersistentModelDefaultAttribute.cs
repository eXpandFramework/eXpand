namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos{
    public interface IPersistentModelDefaultAttribute : IPersistentAttributeInfo {
        string PropertyName { get; set; }
        string Value { get; set; }
    }
}