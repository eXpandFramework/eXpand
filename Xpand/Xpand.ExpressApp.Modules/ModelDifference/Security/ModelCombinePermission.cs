using System.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;


namespace Xpand.ExpressApp.ModelDifference.Security
{
    [NonPersistent]
    public class ModelCombinePermission:PermissionBase
    {
        public ModelCombinePermission(){
        }

        public ModelCombinePermission(ApplicationModelCombineModifier modifier)
        {
            Modifier = modifier;
        }

        public override string ToString()
        {
            return string.Format("{0}({1},{2})", GetType().Name, Modifier,Difference);
        }

        public string Difference { get; set; }

        public ApplicationModelCombineModifier Modifier { get; set; }

        public override IPermission Copy()
        {
            return new ModelCombinePermission(Modifier);
        }
    }
}
