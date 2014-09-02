using DevExpress.ExpressApp.DC;
using DevExpress.Xpo.Metadata;

namespace Xpand.Persistent.Base.General {
    public static class MemberInfoExtensions {
        public static XPMemberInfo GetXPMemberInfo(this IMemberInfo memberInfo    ){
            return memberInfo.MemberTypeInfo.ModelClass().QueryXPClassInfo().FindMember(memberInfo.Name);
        }
    }
}
