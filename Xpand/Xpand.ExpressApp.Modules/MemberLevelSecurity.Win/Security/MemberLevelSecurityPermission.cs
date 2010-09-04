using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.Security
{
    public enum MemberLevelSecurityPermissionModifier
    {
        Allow,Deny
    }
    [NonPersistent]
    public class MemberLevelSecurityPermission:PermissionBase
    {
        public MemberLevelSecurityPermission(MemberLevelSecurityPermissionModifier modifier)
        {
            Modifier = modifier;
        }

        public MemberLevelSecurityPermission()
        {
        }

        public override IPermission Copy()
        {
            return new MemberLevelSecurityPermission();
        }

        public MemberLevelSecurityPermissionModifier Modifier { get; set; }
        public override bool IsSubsetOf(IPermission target)
        {
            return ((MemberLevelSecurityPermission)target).Modifier == Modifier;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), Modifier);
        }

    }
}


