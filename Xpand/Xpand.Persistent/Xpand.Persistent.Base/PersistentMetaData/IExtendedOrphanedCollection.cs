namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedOrphanedCollection : IExtendedCollectionMemberInfo {
        string Criteria { get; set; }
        string ElementTypeFullName { get; set; }
    }
}