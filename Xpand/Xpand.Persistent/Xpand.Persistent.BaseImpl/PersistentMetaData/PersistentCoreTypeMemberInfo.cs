using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData
{
    [InterfaceRegistrator(typeof(IPersistentCoreTypeMemberInfo))]
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