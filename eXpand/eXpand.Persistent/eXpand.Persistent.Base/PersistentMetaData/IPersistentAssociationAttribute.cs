using System;
namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssociationAttribute:IPersistentAttributeInfo {
        string AssociationName { get; set; }
        Type ElementType { get; set; }
    }
}