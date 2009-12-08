using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class ExtendedCoreTypeMemberInfo:ExtendedMemberInfo, IExtendedCoreTypeMemberInfo {
        public ExtendedCoreTypeMemberInfo(Session session) : base(session) {
        }
        XPODataType _dataType;
        public XPODataType DataType
        {
            get { return _dataType; }
            set { SetPropertyValue("Type", ref _dataType, value); }
        }
    }
}