using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class RoleDifferenceObjectBuilder {

        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security) {
            return XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == security.RoleType);
        }

        public static void CreateDynamicRoleMember(ISecurityComplex security) {
            XafTypesInfo.Instance.CreateBothPartMembers(
                GetRoleTypeInfo(security).Type, typeof(RoleModelDifferenceObject),
                XpandModuleBase.Dictiorary, true, "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects", "RoleModelDifferenceObjects", "Roles");
        }
    }
}