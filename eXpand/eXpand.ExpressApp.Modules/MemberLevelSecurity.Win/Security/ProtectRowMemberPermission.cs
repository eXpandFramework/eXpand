using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.Security
{
    public enum ProtectRowMemberPermissionModifier
    {
        Allow,Deny
    }
    [NonPersistent]
    public class ProtectRowMemberPermission:PermissionBase
    {
        public ProtectRowMemberPermission(ProtectRowMemberPermissionModifier modifier)
        {
            Modifier = modifier;
        }

        public ProtectRowMemberPermission()
        {
        }

        public override IPermission Copy()
        {
            return new ProtectRowMemberPermission();
        }

        public ProtectRowMemberPermissionModifier Modifier { get; set; }
        public override bool IsSubsetOf(IPermission target)
        {
            return ((ProtectRowMemberPermission)target).Modifier == Modifier;
        }
        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), Modifier);
        }

    }
}


