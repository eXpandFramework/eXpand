using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    public class PersistentCoreTypeMemberInfo : PersistentMemberInfo, IPersistentCoreTypeMemberInfo {
        public PersistentCoreTypeMemberInfo(Session session) : base(session) { }


        XPODataType _dataType;
        public XPODataType DataType {
            get { return _dataType; }
            set { SetPropertyValue("Type", ref _dataType, value); }
        }

    }
}