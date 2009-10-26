using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    public class ExtendedCollectionMemberInfo:ExtendedMemberInfo,IExtendedCollectionMemberInfo {
        public ExtendedCollectionMemberInfo(Session session) : base(session) {
        }
    }
}