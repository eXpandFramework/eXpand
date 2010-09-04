using DevExpress.Xpo.DB;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentCoreTypeMemberInfo:IPersistentMemberInfo {
        DBColumnType DataType { get; set; }
    }
}