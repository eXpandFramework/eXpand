using DevExpress.ExpressApp.DC;
using DevExpress.Xpo.Metadata;

namespace Xpand.Persistent.Base.General {
    public static class MemberInfoExtensions {
        public static XPMemberInfo GetXPMemberInfo(this IMemberInfo memberInfo    ){
            return memberInfo.MemberTypeInfo.GetModelClass().GetXPClassInfo().FindMember(memberInfo.Name);
        }
    }
}
