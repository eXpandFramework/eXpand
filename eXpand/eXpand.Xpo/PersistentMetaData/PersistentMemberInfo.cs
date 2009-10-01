using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    public abstract class PersistentMemberInfo : PersistentTypeInfo {
        protected PersistentMemberInfo(Session session) : base(session) { }

        internal PersistentMemberInfo()
        {
        }

        internal XPMemberInfo CreateMember(XPClassInfo owner) {
            XPMemberInfo result = owner.FindMember(Name) ?? CreateMemberCore(owner);
            CreateAttributes(result);
            return result;
        }

        protected abstract XPMemberInfo CreateMemberCore(XPClassInfo owner);

        PersistentClassInfo _Owner;
        [Association]
        public PersistentClassInfo Owner {
            get { return _Owner; }
            set { SetPropertyValue("Owner", ref _Owner, value); }
        }
    }
}