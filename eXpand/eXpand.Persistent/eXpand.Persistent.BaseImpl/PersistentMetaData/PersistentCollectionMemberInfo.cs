using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    public class PersistentCollectionMemberInfo : PersistentMemberInfo,IPersistentCollectionMemberInfo {
        public PersistentCollectionMemberInfo(Session session) : base(session) { }

        public PersistentCollectionMemberInfo(PersistentAssociationAttribute persistentAssociationAttribute):this(Session.DefaultSession,persistentAssociationAttribute)
        {
        }

        public PersistentCollectionMemberInfo(Session session, PersistentAssociationAttribute persistentAssociationAttribute)
            : base(session)
        {
            TypeAttributes.Add(persistentAssociationAttribute);
        }

    }
}