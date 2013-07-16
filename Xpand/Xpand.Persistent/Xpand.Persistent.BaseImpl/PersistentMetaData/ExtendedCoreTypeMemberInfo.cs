using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof(IExtendedCoreTypeMemberInfo))]
    public class ExtendedCoreTypeMemberInfo : ExtendedMemberInfo, IExtendedCoreTypeMemberInfo {
        public ExtendedCoreTypeMemberInfo(Session session)
            : base(session) {
        }
        DBColumnType _dataType;
        public DBColumnType DataType {
            get { return _dataType; }
            set { SetPropertyValue("Type", ref _dataType, value); }
        }
    }
}