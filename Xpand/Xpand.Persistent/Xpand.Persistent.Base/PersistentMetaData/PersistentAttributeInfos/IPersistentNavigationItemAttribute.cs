namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentNavigationItemAttribute:IPersistentAttributeInfo {
        string ViewId { get; set; }
        string ObjectKey { get; set; }
        string Path { get; set; }
    }
}