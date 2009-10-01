using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    public class PersistentCoreTypeMemberInfo : PersistentMemberInfo {
        public PersistentCoreTypeMemberInfo(Session session) : base(session) { }

        public PersistentCoreTypeMemberInfo()
        {
        }

        string _TypeName;
        public string TypeName {
            get { return _TypeName; }
            set { SetPropertyValue("TypeName", ref _TypeName, value); }
        }

        protected override XPMemberInfo CreateMemberCore(XPClassInfo owner) {
            return owner.CreateMember(Name, Type.GetType(TypeName, true));
        }
    }
}