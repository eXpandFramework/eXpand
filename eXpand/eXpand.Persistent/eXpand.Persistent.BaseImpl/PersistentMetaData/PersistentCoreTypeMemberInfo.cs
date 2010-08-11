using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    [WCRegistrator(typeof(IPersistentCoreTypeMemberInfo))]
    public class PersistentCoreTypeMemberInfo : PersistentMemberInfo, IPersistentCoreTypeMemberInfo {
        public PersistentCoreTypeMemberInfo(Session session) : base(session) { }


        DBColumnType _dataType;
        public DBColumnType DataType
        {
            get { return _dataType; }
            set { SetPropertyValue("Type", ref _dataType, value); }
        }

    }
}