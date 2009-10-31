using eXpand.Xpo;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedCoreTypeMemberInfo:IExtendedMemberInfo {
        XPODataType DataType { get; set; }
    }
}