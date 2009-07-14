using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    public class PersistentReferenceMemberInfo : PersistentMemberInfo {
        public PersistentReferenceMemberInfo(Session session) : base(session) { }

        public PersistentReferenceMemberInfo()
        {
        }

        public PersistentReferenceMemberInfo(Session session,PersistentAssociationAttribute persistentAssociationAttribute) : base(session)
        {
            TypeAttributes.Add(persistentAssociationAttribute);
        }

        PersistentClassInfo _ReferenceType;
        public PersistentClassInfo ReferenceType {
            get { return _ReferenceType; }
            set { SetPropertyValue("ReferenceType", ref _ReferenceType, value); }
        }

        protected override XPMemberInfo CreateMemberCore(XPClassInfo owner) {
            XPMemberInfo member = owner.CreateMember(Name, ReferenceType.CreateClass(owner.Dictionary));
            return member;
        }
    }
}