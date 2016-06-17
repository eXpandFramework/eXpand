using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public interface IPersistentAssociationAttribute:IPersistentAttributeInfo {
        RelationType RelationType { get; set; }
        string AssociationName { get; set; }
        string ElementTypeFullName { get; set; }
    }
}