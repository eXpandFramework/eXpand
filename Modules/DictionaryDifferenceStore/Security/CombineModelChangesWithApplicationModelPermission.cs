using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Permissions;


namespace eXpand.ExpressApp.DictionaryDifferenceStore.Security
{
    public enum CombineModelChangesWithApplicationModelModifier
    {
        Allow,Deny
    }
    [NonPersistent]
    public class CombineModelChangesWithApplicationModelPermission:PermissionBase
    {
        public CombineModelChangesWithApplicationModelPermission()
        {
        }

        public CombineModelChangesWithApplicationModelPermission(CombineModelChangesWithApplicationModelModifier modifier)
        {
            Modifier = modifier;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), Modifier);
        }

        public CombineModelChangesWithApplicationModelModifier Modifier { get; set; }

        public override IPermission Copy()
        {
            return new CombineModelChangesWithApplicationModelPermission(Modifier);
        }
    }
}
