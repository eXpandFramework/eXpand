using System.ComponentModel;
using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.SystemModule
{
    public class MemberInfoEventArgs : HandledEventArgs
    {
        private readonly IMemberInfo memberInfo;

        public MemberInfoEventArgs(IMemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public IMemberInfo MemberInfo
        {
            get { return memberInfo; }
        }
    }
}