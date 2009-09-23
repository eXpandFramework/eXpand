using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Permissions;


namespace eXpand.ExpressApp.ModelDifference.Security
{
    [NonPersistent]
    public class ApplicationModelCombinePermission:PermissionBase
    {
        public ApplicationModelCombinePermission(){
        }

        public ApplicationModelCombinePermission(ApplicationModelCombineModifier modifier)
        {
            Modifier = modifier;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), Modifier);
        }

        public ApplicationModelCombineModifier Modifier { get; set; }

        public override IPermission Copy()
        {
            return new ApplicationModelCombinePermission(Modifier);
        }
    }
}
