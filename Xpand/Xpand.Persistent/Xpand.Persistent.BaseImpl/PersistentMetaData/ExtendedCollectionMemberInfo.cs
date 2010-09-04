using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof(IExtendedCollectionMemberInfo))]
    public class ExtendedCollectionMemberInfo:ExtendedMemberInfo,IExtendedCollectionMemberInfo {
        public ExtendedCollectionMemberInfo(Session session) : base(session) {
        }
    }
}