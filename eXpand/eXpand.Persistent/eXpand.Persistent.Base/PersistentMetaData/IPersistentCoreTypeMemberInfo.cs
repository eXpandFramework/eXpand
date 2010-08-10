using DevExpress.Xpo.DB;
using eXpand.Xpo;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentCoreTypeMemberInfo:IPersistentMemberInfo {
        DBColumnType DataType { get; set; }
    }
}