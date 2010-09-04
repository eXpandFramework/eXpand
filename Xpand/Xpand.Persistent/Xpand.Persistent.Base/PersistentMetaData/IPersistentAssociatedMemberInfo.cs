using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssociatedMemberInfo : IPersistentMemberInfo {
        RelationType RelationType { get; set; }
    }
}