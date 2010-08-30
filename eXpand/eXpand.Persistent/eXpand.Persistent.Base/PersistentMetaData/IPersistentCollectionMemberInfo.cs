namespace eXpand.Persistent.Base.PersistentMetaData
{
    public interface IPersistentCollectionMemberInfo: IPersistentAssociatedMemberInfo {
        string CollectionTypeFullName { get; set; }
        void SetCollectionTypeFullName(string s);
        IPersistentClassInfo CollectionClassInfo { get; set; }
    }
}
