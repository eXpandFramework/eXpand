using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class RoleDifferenceObjectBuilder {

        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security) {
            return XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type == security.RoleType).Single();
        }

        public static void CreateDynamicRoleMember(ISecurityComplex security) {
            XafTypesInfo.Instance.CreateBothPartMembers(
                GetRoleTypeInfo(security).Type, typeof(RoleModelDifferenceObject),
                XafTypesInfo.XpoTypeInfoSource.XPDictionary, true, "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects", "RoleModelDifferenceObjects", "Roles");
        }
    }
}