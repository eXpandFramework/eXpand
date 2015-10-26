using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData{
    [InterfaceRegistrator(typeof (IPersistentCoreTypeMemberInfo))]
    [System.ComponentModel.DisplayName("Core")]
    public class PersistentCoreTypeMemberInfo : PersistentMemberInfo, IPersistentCoreTypeMemberInfo{
        private DBColumnType _dataType;

        public PersistentCoreTypeMemberInfo(Session session) : base(session){
        }

        public DBColumnType DataType{
            get { return _dataType; }
            set { SetPropertyValue("Type", ref _dataType, value); }
        }
    }
}