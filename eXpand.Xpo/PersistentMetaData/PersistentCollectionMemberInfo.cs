using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    public class PersistentCollectionMemberInfo : PersistentMemberInfo {
        public PersistentCollectionMemberInfo(Session session) : base(session) { }

        public PersistentCollectionMemberInfo(PersistentAssociationAttribute persistentAssociationAttribute):this(Session.DefaultSession,persistentAssociationAttribute)
        {
        }

        public PersistentCollectionMemberInfo(Session session, PersistentAssociationAttribute persistentAssociationAttribute)
            : base(session)
        {
            TypeAttributes.Add(persistentAssociationAttribute);
        }

        protected override XPMemberInfo CreateMemberCore(XPClassInfo owner) {
            return owner.CreateMember(Name, typeof(XPCollection), true);
        }
    }
}