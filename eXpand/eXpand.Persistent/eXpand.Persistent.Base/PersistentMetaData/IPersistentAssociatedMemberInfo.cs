using eXpand.Persistent.Base.General;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssociatedMemberInfo : IPersistentMemberInfo {
        RelationType RelationType { get; set; }
    }
}