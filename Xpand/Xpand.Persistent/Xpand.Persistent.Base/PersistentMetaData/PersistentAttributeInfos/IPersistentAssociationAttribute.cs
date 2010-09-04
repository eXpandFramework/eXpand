namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAssociationAttribute:IPersistentAttributeInfo {
        string AssociationName { get; set; }
        string ElementTypeFullName { get; set; }
    }
}