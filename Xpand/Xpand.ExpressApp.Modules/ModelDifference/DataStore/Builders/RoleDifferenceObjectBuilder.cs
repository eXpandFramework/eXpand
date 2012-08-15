using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class RoleDifferenceObjectBuilder {

        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security) {
            return XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == security.RoleType);
        }

        public static void CreateDynamicRoleMember(ISecurityComplex security) {
            var typeToCreateOn = GetRoleTypeInfo(security).Type;
            var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(typeToCreateOn, typeof(RoleModelDifferenceObject), XpandModuleBase.Dictiorary, true, "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects", "RoleModelDifferenceObjects", "Roles");
            xpCustomMemberInfos.First(info => info.Name == "RoleModelDifferenceObjects").AddAttribute(new VisibleInDetailViewAttribute(false));
            XafTypesInfo.Instance.RefreshInfo(typeToCreateOn);
        }
    }
}