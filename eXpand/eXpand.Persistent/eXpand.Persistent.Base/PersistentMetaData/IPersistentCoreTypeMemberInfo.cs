using eXpand.Xpo;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentCoreTypeMemberInfo:IPersistentMemberInfo {
        XPODataType DataType { get; set; }
    }
}