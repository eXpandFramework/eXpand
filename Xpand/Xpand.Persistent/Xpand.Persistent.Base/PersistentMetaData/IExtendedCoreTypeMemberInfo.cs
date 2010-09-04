using DevExpress.Xpo.DB;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedCoreTypeMemberInfo:IExtendedMemberInfo {
        DBColumnType DataType { get; set; }
    }
}