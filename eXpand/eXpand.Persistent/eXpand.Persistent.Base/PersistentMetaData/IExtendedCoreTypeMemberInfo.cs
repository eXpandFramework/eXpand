using DevExpress.Xpo.DB;
using eXpand.Xpo;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedCoreTypeMemberInfo:IExtendedMemberInfo {
        DBColumnType DataType { get; set; }
    }
}