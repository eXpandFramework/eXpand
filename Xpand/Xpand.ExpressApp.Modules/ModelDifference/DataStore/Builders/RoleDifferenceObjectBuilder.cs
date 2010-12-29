using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class RoleDifferenceObjectBuilder {

        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security) {
            return XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type == security.RoleType).Single();
        }

        public static void CreateDynamicMembers(ISecurityComplex security) {
            XafTypesInfo.Instance.CreateBothPartMembers(
                GetRoleTypeInfo(security).Type, typeof(RoleModelDifferenceObject),
                XafTypesInfo.XpoTypeInfoSource.XPDictionary, true, "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects", "RoleModels", "Roles");
        }
    }
}