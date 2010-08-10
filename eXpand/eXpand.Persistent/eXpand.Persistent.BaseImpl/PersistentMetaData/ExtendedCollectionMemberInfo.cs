using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [Registrator(typeof(IExtendedCollectionMemberInfo))]
    public class ExtendedCollectionMemberInfo:ExtendedMemberInfo,IExtendedCollectionMemberInfo {
        public ExtendedCollectionMemberInfo(Session session) : base(session) {
        }
    }
}